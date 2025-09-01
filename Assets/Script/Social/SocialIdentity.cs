using UnityEngine;

public static class SocialIdentity
{
    public static string ProfileId =>
        PlayerInventory.Instance != null ? PlayerInventory.Instance.CurrentProfileId : "DEFAULT";

    const string NameKey = "player_name";

    // Oyuncu ad�: �nce PlayerPrefs -> yoksa ProfileId -> yoksa "Player"
    public static string PlayerName
    {
        get
        {
            var n = PlayerPrefs.GetString(NameKey, null);
            if (!string.IsNullOrWhiteSpace(n)) return n;

            var pid = (PlayerInventory.Instance != null) ? PlayerInventory.Instance.CurrentProfileId : null;
            if (!string.IsNullOrWhiteSpace(pid)) return pid;

            return "Player";
        }
    }

    // �stedi�in yerden (�r. LocalLogin ba�ar�l� oldu�unda) �a��r:
    public static void SetPlayerName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        PlayerPrefs.SetString(NameKey, name);
        PlayerPrefs.Save();
    }
}
