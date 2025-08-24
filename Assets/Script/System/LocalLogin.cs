using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalLogin : MonoBehaviour
{
    [Header("Panels")]
    public GameObject loginPanel;          // Sadece Login UI
    public GameObject lobbyPanel;          // Lobby (login'den sonra otomatik a��lmayacak)

    [Tooltip("Giri�ten SONRA g�r�nmesini istedi�in ekstra UI k�kleri (�rn: alt buton bar�n�n parent'�).")]
    public List<GameObject> showAfterLogin = new List<GameObject>();

    [Header("Inputs & Buttons")]
    public InputField usernameInput;
    public InputField passwordInput;
    public Button loginButton;
    public Button logoutButton;
    public Button saveButton;

    [Header("Options")]
    [Tooltip("Uygulama a��ld���nda son profil ile otomatik giri� denesin mi?")]
    public bool autoLoginOnStart = true;

    // PlayerPrefs anahtarlar�
    const string Key_ProfileId = "PROFILE_ID";
    const string Key_Remember = "REMEMBER_ME";

    void Awake()
    {
        // �lk karede sadece LoginPanel a��k kals�n
        SetActiveSafe(loginPanel, true);
        SetActiveSafe(lobbyPanel, false);
        SetListActive(showAfterLogin, false);

        // Buton ba�lar�
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
            // Sessiz Login: paneli kapat, UI�lar� a�, envanter y�kle
            DoLogin(pid, silent: true);
        }
    }

    // ----------------- UI helpers -----------------
    void SetActiveSafe(GameObject go, bool v) { if (go) go.SetActive(v); }
    void SetListActive(List<GameObject> list, bool v) { foreach (var go in list) SetActiveSafe(go, v); }

    void ShowOnlyLoginPanel(bool show)
    {
        SetActiveSafe(loginPanel, show);
        // Login ekran� a��kken Lobby ve di�erleri kapal�
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

        // Bu �rnekte �ifre kontrol� yok. �stersen burada do�rulama yap.
        if (string.IsNullOrEmpty(user)) user = "DEFAULT";

        DoLogin(user, silent: false);
    }

    void OnClick_Logout()
    {
        // Kaydet (opsiyonel)
        TrySaveInventory();

        // �remember me� temizle (otomatik giri� istemeyebiliriz)
        PlayerPrefs.SetInt(Key_Remember, 0);
        PlayerPrefs.Save();

        // UI: sadece Login ekran� a��k kals�n
        ShowOnlyLoginPanel(true);
    }

    void OnClick_Save()
    {
        TrySaveInventory();
    }

    // ----------------- Core login/save/load -----------------
    void DoLogin(string profileId, bool silent)
    {
        // Profil ID�yi hat�rla (otomatik giri�)
        PlayerPrefs.SetString(Key_ProfileId, profileId);
        PlayerPrefs.SetInt(Key_Remember, 1);
        PlayerPrefs.Save();

        // Profil ID�yi oyunun state�ine yaz
        PlayerInventory.CurrentProfileId = profileId;

        // Envanteri bu profilden y�kle (varsa)
        TryLoadInventory(profileId);

        // UI: Login panel kapan�r, Lobby A�ILMAZ (istemiyoruz),
        // sadece �showAfterLogin� listesi a��l�r (�rn: alt buton bar�).
        ShowOnlyLoginPanel(false);
        SetListActive(showAfterLogin, true);
        // lobbyPanel bilin�li olarak kapal� b�rak�l�yor.
    }

    void TryLoadInventory(string profileId)
    {
        var inv = FindObjectOfType<PlayerInventory>();
        if (inv == null) return;

        PlayerInventory.CurrentProfileId = profileId;
        inv.LoadFromDisk();
        Debug.Log($"[LocalLogin] Envanter y�klendi (profil={profileId}).");
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
