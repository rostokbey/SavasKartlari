using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalLogin : MonoBehaviour
{
    [Header("Panels")]
    public GameObject loginPanel;          // Sadece Login UI
    public GameObject lobbyPanel;          // Lobby (login'den sonra otomatik açýlmayacak)

    [Tooltip("Giriþten SONRA görünmesini istediðin ekstra UI kökleri (örn: alt buton barýnýn parent'ý).")]
    public List<GameObject> showAfterLogin = new List<GameObject>();

    [Header("Inputs & Buttons")]
    public InputField usernameInput;
    public InputField passwordInput;
    public Button loginButton;
    public Button logoutButton;
    public Button saveButton;

    [Header("Options")]
    [Tooltip("Uygulama açýldýðýnda son profil ile otomatik giriþ denesin mi?")]
    public bool autoLoginOnStart = true;

    // PlayerPrefs anahtarlarý
    const string Key_ProfileId = "PROFILE_ID";
    const string Key_Remember = "REMEMBER_ME";

    void Awake()
    {
        // Ýlk karede sadece LoginPanel açýk kalsýn
        SetActiveSafe(loginPanel, true);
        SetActiveSafe(lobbyPanel, false);
        SetListActive(showAfterLogin, false);

        // Buton baðlarý
        if (loginButton) loginButton.onClick.AddListener(OnClick_Login);
        if (logoutButton) logoutButton.onClick.AddListener(OnClick_Logout);
        if (saveButton) saveButton.onClick.AddListener(OnClick_Save);
    }

    void Start()
    {
        if (!autoLoginOnStart) return;

        var remember = PlayerPrefs.GetInt(Key_Remember, 0) == 1;
        var pid = PlayerPrefs.GetString(Key_ProfileId, "DEFAULT");

        if (remember && !string.IsNullOrEmpty(pid))
        {
            // Sessiz Login: paneli kapat, UI’larý aç, envanter yükle
            DoLogin(pid, silent: true);
        }
    }

    // ----------------- UI helpers -----------------
    void SetActiveSafe(GameObject go, bool v) { if (go) go.SetActive(v); }
    void SetListActive(List<GameObject> list, bool v) { foreach (var go in list) SetActiveSafe(go, v); }

    void ShowOnlyLoginPanel(bool show)
    {
        SetActiveSafe(loginPanel, show);
        // Login ekraný açýkken Lobby ve diðerleri kapalý
        if (show)
        {
            SetActiveSafe(lobbyPanel, false);
            SetListActive(showAfterLogin, false);
        }
    }

    // ----------------- Button handlers -----------------
    void OnClick_Login()
    {
        string user = usernameInput ? usernameInput.text.Trim() : "DEFAULT";
        string pass = passwordInput ? passwordInput.text.Trim() : "";

        // Bu örnekte þifre kontrolü yok. Ýstersen burada doðrulama yap.
        if (string.IsNullOrEmpty(user)) user = "DEFAULT";

        DoLogin(user, silent: false);
    }

    void OnClick_Logout()
    {
        // Kaydet (opsiyonel)
        TrySaveInventory();

        // “remember me” temizle (otomatik giriþ istemeyebiliriz)
        PlayerPrefs.SetInt(Key_Remember, 0);
        PlayerPrefs.Save();

        // UI: sadece Login ekraný açýk kalsýn
        ShowOnlyLoginPanel(true);
    }

    void OnClick_Save()
    {
        TrySaveInventory();
    }

    // ----------------- Core login/save/load -----------------
    void DoLogin(string profileId, bool silent)
    {
        // Profil ID’yi hatýrla (otomatik giriþ)
        PlayerPrefs.SetString(Key_ProfileId, profileId);
        PlayerPrefs.SetInt(Key_Remember, 1);
        PlayerPrefs.Save();

        // Profil ID’yi oyunun state’ine yaz
        PlayerInventory.CurrentProfileId = profileId;

        // Envanteri bu profilden yükle (varsa)
        TryLoadInventory(profileId);

        // UI: Login panel kapanýr, Lobby AÇILMAZ (istemiyoruz),
        // sadece “showAfterLogin” listesi açýlýr (örn: alt buton barý).
        ShowOnlyLoginPanel(false);
        SetListActive(showAfterLogin, true);
        // lobbyPanel bilinçli olarak kapalý býrakýlýyor.
    }

    void TryLoadInventory(string profileId)
    {
        var inv = FindObjectOfType<PlayerInventory>();
        if (inv == null) return;

        PlayerInventory.CurrentProfileId = profileId;
        inv.LoadFromDisk();
        Debug.Log($"[LocalLogin] Envanter yüklendi (profil={profileId}).");
    }

    void TrySaveInventory()
    {
        var inv = FindObjectOfType<PlayerInventory>();
        if (inv == null) return;

        string pid = string.IsNullOrEmpty(PlayerInventory.CurrentProfileId) ? "DEFAULT" : PlayerInventory.CurrentProfileId;
        inv.SaveToDisk();
        Debug.Log($"[LocalLogin] Envanter kaydedildi: {pid}");
    }
}
