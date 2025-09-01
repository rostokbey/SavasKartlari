using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SeasonHUD : MonoBehaviour
{
    public static SeasonHUD Instance { get; private set; }

    // UI refs
    Canvas canvas;
    RectTransform root;
    TextMeshProUGUI lblTitle, lblPts, lblWL, lblStreak, lblLeft;

    // colors
    readonly Color32 colPanel = new Color32(26, 26, 26, 220);
    readonly Color32 colText = new Color32(235, 235, 235, 255);
    readonly Color32 colSub = new Color32(200, 200, 200, 220);

    // ---------------------------------------------------------------------
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoBoot()
    {
        // sahnede yoksa otomatik oluştur
        if (FindObjectOfType<SeasonHUD>() == null)
        {
            var go = new GameObject("SeasonHUD_Runtime");
            go.AddComponent<SeasonHUD>();
            DontDestroyOnLoad(go);
        }
    }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        BuildUI();
        StartCoroutine(RefreshLoop());
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // ---------------------------------------------------------------------
    void BuildUI()
    {
        // Canvas + Scaler
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        gameObject.AddComponent<GraphicRaycaster>();

        // kök panel (sağ üst)
        root = CreateRect(transform, "Root");
        root.anchorMin = new Vector2(1, 1);
        root.anchorMax = new Vector2(1, 1);
        root.pivot = new Vector2(1, 1);
        root.anchoredPosition = new Vector2(-18, -18);
        root.sizeDelta = new Vector2(280, 120);

        var bg = root.gameObject.AddComponent<Image>();
        bg.color = colPanel;

        var v = root.gameObject.AddComponent<VerticalLayoutGroup>();
        v.padding = new RectOffset(10, 10, 10, 10);
        v.childAlignment = TextAnchor.UpperRight;
        v.spacing = 2;
        v.childControlWidth = v.childControlHeight = true;

        // üstte küçük toggle etiketi
        var top = CreateRect(root, "Top", 0, 0, new Vector2(0, 20));
        var topH = top.gameObject.AddComponent<HorizontalLayoutGroup>();
        topH.childAlignment = TextAnchor.MiddleRight;
        topH.childControlWidth = topH.childControlHeight = true;

        var hudBtn = CreateLabel(top, "HUD", 14, FontStyles.Bold);
        hudBtn.color = colSub;
        var btn = hudBtn.gameObject.AddComponent<Button>();
        btn.onClick.AddListener(() => Toggle());

        // başlık
        lblTitle = CreateLabel(root, "Season", 18, FontStyles.Bold);
        lblTitle.color = colText;

        // satırlar
        lblPts = CreateLabel(root, "Pts", 16, FontStyles.Normal);
        lblWL = CreateLabel(root, "W/L", 16, FontStyles.Normal);
        lblStreak = CreateLabel(root, "Streak", 16, FontStyles.Normal);
        lblLeft = CreateLabel(root, "Left", 14, FontStyles.Italic);
        lblLeft.color = colSub;

        UpdateNow();
    }

    IEnumerator RefreshLoop()
    {
        var wait = new WaitForSeconds(1f);
        while (true)
        {
            UpdateNow();
            yield return wait;
        }
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.F2)) Toggle();
#endif
    }

    void Toggle() => gameObject.SetActive(!gameObject.activeSelf);

    // ---------------------------------------------------------------------
    void UpdateNow()
    {
        // güvenli erişim – SeasonManager/Repository yoksa sessizce çıkar
        var sm = SeasonManager.Instance;
        if (sm == null)
        {
            lblTitle.text = "Season (offline)";
            lblPts.text = lblWL.text = lblStreak.text = lblLeft.text = "";
            return;
        }

        // puan ve sayaçlar
        int pts = sm.GetMyPoints();
        int wStreak = sm.GetMyWinStreak();
        int lStreak = sm.GetMyLossStreak();
        bool elim = sm.IsEliminated();
        var left = sm.GetTimeLeft(); // TimeSpan

        lblTitle.text = elim ? "Season (ELIMINATED)" : "Season";

        lblPts.text = $"Pts: <b>{pts}</b>";
        // (wins/losses) SeasonRepository’dan çekiyoruz
        var entry = SeasonRepository.GetEntry(GetProfileId() ?? "DEFAULT");
        int wins = entry?.wins ?? 0;
        int loss = entry?.losses ?? 0;
        lblWL.text = $"W/L: <b>{wins}</b>/<b>{loss}</b>";
        lblStreak.text = $"Streak: W{wStreak} / L{lStreak}";
        lblLeft.text = $"Left: {Format(left)}";
    }

    // ---------------------------------------------------------------------
    // helpers
    static RectTransform CreateRect(Transform parent, string name, float ax = 1, float ay = 1, Vector2? size = null)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(ax, ay);
        rt.anchorMax = new Vector2(ax, ay);
        rt.pivot = new Vector2(ax, ay);
        if (size.HasValue) rt.sizeDelta = size.Value;
        return rt;
    }

    static TextMeshProUGUI CreateLabel(Transform parent, string text, float size, FontStyles style)
    {
        var go = new GameObject(text, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.font = TMP_Settings.defaultFontAsset;
        t.text = text;
        t.fontSize = size;
        t.fontStyle = style;
        t.alignment = TextAlignmentOptions.MidlineRight;
        t.enableWordWrapping = false;
        t.color = Color.white;
        return t;
    }

    static string GetProfileId()
    {
        var inv = PlayerInventory.Instance ?? Object.FindObjectOfType<PlayerInventory>();
        return inv != null ? inv.CurrentProfileId : null;
    }

    static string Format(System.TimeSpan ts)
    {
        if (ts.TotalDays >= 1) return $"{(int)ts.TotalDays}d {ts.Hours}h";
        if (ts.TotalHours >= 1) return $"{(int)ts.TotalHours}h {ts.Minutes}m";
        return $"{Mathf.Max(0, ts.Minutes)}m {Mathf.Max(0, ts.Seconds)}s";
    }
}
