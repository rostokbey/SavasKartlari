
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LocalLogin : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;

    void Start()
    {
        loginButton.onClick.AddListener(OnLogin);
    }

    static string Hash(string s)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(s));
        var sb = new StringBuilder();
        foreach (var b in bytes) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    void OnLogin()
    {
        var user = usernameInput.text.Trim();
        var pass = passwordInput.text;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            Debug.LogWarning("Kullan�c� ad�/�ifre bo� olamaz.");
            return;
        }

        var key = $"auth_{user}";
        var storedHash = PlayerPrefs.GetString(key, "");

        var incomingHash = Hash(pass);

        if (string.IsNullOrEmpty(storedHash))
        {
            // �lk kez kay�t
            PlayerPrefs.SetString(key, incomingHash);
            PlayerPrefs.Save();
            Debug.Log("Yeni kullan�c� olu�turuldu: " + user);
        }
        else
        {
            if (storedHash != incomingHash)
            {
                Debug.LogError("�ifre hatal�!");
                return;
            }
        }

        // Aktif profil olarak kullan�c� ad�
        PlayerInventory.CurrentProfileId = user;

        // Envanteri bu profile g�re yeniden y�kle
        PlayerInventory.Instance.LoadFromDisk();

        // Login panelini kapat (istersen)
        gameObject.SetActive(false);
    }
}
