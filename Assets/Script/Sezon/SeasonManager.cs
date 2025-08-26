using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// 3 haftalık sezon, puan akışı ve ödüller:
/// * Start: 1000 puan
/// * Win +50, Loss -25
/// * 3+ loss streak → ekstra ceza (loss çarpanı)
/// * Puan 0 → sezon için ELENİR
/// * Sezon sonunda SADECE 1.'ye madalya (5 madalya=1 kupa, 5 kupa=1 yıldız)
/// </summary>
public class SeasonManager : MonoBehaviour
{
    public static SeasonManager Instance { get; private set; }

    [Header("Sezon Süresi")]
    [SerializeField] private int seasonDurationDays = 21;  // 3 hafta

    [Header("Puan Kuralları")]
    [SerializeField] private int startPoints = 500;
    [SerializeField] private int winDelta = 50;
    [SerializeField] private int lossDelta = -25;

    [Header("Loss Streak Cezası")]
    [SerializeField] private int lossStreakThreshold = 3;    // 3. yenilgiden itibaren
    [SerializeField] private float lossStreakPenaltyMultiplier = 2f;   // kayıp ×2

    [Header("Sezon Kimliği")]
    [SerializeField] private string seasonId = "S1";

    [Header("Durum (Debug)")]
    [SerializeField] private DateTime seasonStartUtc;
    [SerializeField] private DateTime seasonEndUtc;

    private Dictionary<string, PlayerSeasonState> stateByProfile = new Dictionary<string, PlayerSeasonState>();

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        if (seasonStartUtc == default)
        {
            seasonStartUtc = DateTime.UtcNow;
            seasonEndUtc = seasonStartUtc.AddDays(seasonDurationDays);
        }
        LoadSeason();
    }

    // ---------------------------------------------------------------------
    // Dış API
    // ---------------------------------------------------------------------

    public void StartNewSeason(string newSeasonId = null)
    {
        if (!string.IsNullOrEmpty(newSeasonId))
            seasonId = newSeasonId;

        seasonStartUtc = DateTime.UtcNow;
        seasonEndUtc = seasonStartUtc.AddDays(seasonDurationDays);
        stateByProfile.Clear();
        SaveSeason();

        Debug.Log($"[Season] Yeni sezon başladı: {seasonId}  ({seasonStartUtc:u} → {seasonEndUtc:u})");
    }

    /// <summary>
    /// Maç bitişinde çağır. myTeamWon: kazandın mı? teamSize: 1/2/3 (şimdilik puanı etkilemiyor).
    /// Puanı 0 olan (elenmiş) oyuncu yeni maçta puan alamaz.
    /// </summary>
    public void OnMatchFinished(bool myTeamWon, int teamSize = 1)
    {
        string pid = EnsureProfile();
        var st = GetOrCreate(pid);

        if (st.isEliminated)
        {
            Debug.Log($"[Season] {pid} elenmiş, maç sonucu işlenmedi.");
            return;
        }

        int delta = myTeamWon ? winDelta : lossDelta;

        if (myTeamWon)
        {
            st.lossStreak = 0; // streak sıfırla
        }
        else
        {
            st.lossStreak++;
            if (st.lossStreak >= lossStreakThreshold)
            {
                // ek ceza: lossDelta negatif olduğu için mutlak değeri büyüt
                delta = Mathf.RoundToInt(delta * lossStreakPenaltyMultiplier);
            }
        }

        st.points = Mathf.Max(0, st.points + delta);
        st.totalMatches++;
        if (myTeamWon) st.wins++; else st.losses++;

        // 0'a düşerse elenir
        if (st.points <= 0)
        {
            st.points = 0;
            st.isEliminated = true;
            Debug.Log($"[Season] {pid} ELENDİ (puan=0).");
        }

        stateByProfile[pid] = st;
        SaveSeason();

        Debug.Log($"[Season] {pid} → {(delta >= 0 ? "+" : "")}{delta}  (Toplam: {st.points}, LS:{st.lossStreak})");
    }

    /// <summary>
    /// Sezon biter: yalnızca 1.'ye madalya ver; 5 madalya=1 kupa, 5 kupa=1 yıldız.
    /// </summary>
    public List<PlayerSeasonState> EndSeasonAndDistributeRewards()
    {
        var standings = BuildStandings();

        if (standings.Count > 0)
        {
            standings[0].medals++;
            Consolidate(standings[0]);
            Debug.Log($"[Season] 1.si: {standings[0].profileId} → +1 madalya (M:{standings[0].medals} K:{standings[0].cups} Y:{standings[0].stars})");
        }

        // 2. ve 3.'ye artık ödül yok

        foreach (var s in standings)
            stateByProfile[s.profileId] = s;

        SaveSeason();
        return standings;
    }

    /// <summary>Aktif profil puanı.</summary>
    public int GetMyPoints()
    {
        var pid = EnsureProfile();
        return GetOrCreate(pid).points;
    }

    /// <summary>Oyuncu elenmiş mi?</summary>
    public bool IsEliminated(string profileId = null)
    {
        var pid = profileId ?? EnsureProfile();
        return GetOrCreate(pid).isEliminated;
    }

    /// <summary>Basit sıralama (puan desc, sonra wins, sonra totalMatches).</summary>
    public List<PlayerSeasonState> BuildStandings()
    {
        var list = new List<PlayerSeasonState>(stateByProfile.Values);
        list.Sort((a, b) =>
        {
            int cmp = b.points.CompareTo(a.points);
            if (cmp != 0) return cmp;
            cmp = b.wins.CompareTo(a.wins);
            if (cmp != 0) return cmp;
            return b.totalMatches.CompareTo(a.totalMatches);
        });
        return list;
    }

    // ---------------------------------------------------------------------
    // İç
    // ---------------------------------------------------------------------

    private string EnsureProfile()
    {
        var inv = PlayerInventory.Instance ?? FindObjectOfType<PlayerInventory>();
        string pid = (inv != null && !string.IsNullOrEmpty(inv.CurrentProfileId)) ? inv.CurrentProfileId : "DEFAULT";
        return pid;
    }

    private PlayerSeasonState GetOrCreate(string profileId)
    {
        if (!stateByProfile.TryGetValue(profileId, out var st))
        {
            st = new PlayerSeasonState
            {
                profileId = profileId,
                points = startPoints,
                wins = 0,
                losses = 0,
                totalMatches = 0,
                medals = 0,
                cups = 0,
                stars = 0,
                lossStreak = 0,
                isEliminated = false
            };
            stateByProfile[profileId] = st;
        }
        return st;
    }

    // 5 madalya -> 1 kupa; 5 kupa -> 1 yıldız
    private void Consolidate(PlayerSeasonState st)
    {
        if (st.medals >= 5)
        {
            st.cups += st.medals / 5;
            st.medals = st.medals % 5;
        }
        if (st.cups >= 5)
        {
            st.stars += st.cups / 5;
            st.cups = st.cups % 5;
        }
    }

    private string SeasonPath()
    {
        var dir = Application.persistentDataPath;
        return Path.Combine(dir, $"season_{seasonId}.json");
    }

    private void SaveSeason()
    {
        var data = new SeasonSaveData
        {
            seasonId = seasonId,
            seasonStartUtc = seasonStartUtc.ToUniversalTime().Ticks,
            seasonEndUtc = seasonEndUtc.ToUniversalTime().Ticks,
            players = new List<PlayerSeasonState>(stateByProfile.Values)
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(SeasonPath(), json, Encoding.UTF8);
    }

    private void LoadSeason()
    {
        var path = SeasonPath();
        if (!File.Exists(path))
        {
            GetOrCreate(EnsureProfile());
            SaveSeason();
            return;
        }

        var json = File.ReadAllText(path, Encoding.UTF8);
        var data = JsonUtility.FromJson<SeasonSaveData>(json);
        stateByProfile.Clear();

        if (data != null)
        {
            seasonId = data.seasonId ?? seasonId;
            if (data.seasonStartUtc > 0) seasonStartUtc = new DateTime(data.seasonStartUtc, DateTimeKind.Utc);
            if (data.seasonEndUtc > 0) seasonEndUtc = new DateTime(data.seasonEndUtc, DateTimeKind.Utc);

            if (data.players != null)
                foreach (var p in data.players)
                    stateByProfile[p.profileId] = p;
        }
    }

    public int GetMyLossStreak()
    {
        var pid = EnsureProfile();
        return GetOrCreate(pid).lossStreak;
    }

    public TimeSpan GetTimeLeft()
    {
        return seasonEndUtc.ToUniversalTime() - DateTime.UtcNow;
    }

    // (UI’da tarih istemezsen şart değil ama işine yarar)
    public DateTime SeasonEndUtc => seasonEndUtc;

}

// ---------------- DTO'lar ----------------

[Serializable]
public class SeasonSaveData
{
    public string seasonId;
    public long seasonStartUtc;
    public long seasonEndUtc;
    public List<PlayerSeasonState> players = new List<PlayerSeasonState>();
}

[Serializable]
public class PlayerSeasonState
{
    public string profileId;
    public int points;
    public int wins;
    public int losses;
    public int totalMatches;

    public int medals;
    public int cups;
    public int stars;

    public int lossStreak;    // arka arkaya kayıp sayısı
    public bool isEliminated;  // puan 0'a düştü mü?
}
