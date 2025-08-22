
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
            Debug.LogWarning("Kullanýcý adý/þifre boþ olamaz.");
            return;
        }

        var key = $"auth_{user}";
        var storedHash = PlayerPrefs.GetString(key, "");

        var incomingHash = Hash(pass);

        if (string.IsNullOrEmpty(storedHash))
        {
            // Ýlk kez kayýt
            PlayerPrefs.SetString(key, incomingHash);
            PlayerPrefs.Save();
            Debug.Log("Yeni kullanýcý oluþturuldu: " + user);
        }
        else
        {
            if (storedHash != incomingHash)
            {
                Debug.LogError("Þifre hatalý!");
                return;
            }
        }

        // Aktif profil olarak kullanýcý adý
        PlayerInventory.CurrentProfileId = user;

        // Envanteri bu profile göre yeniden yükle
        PlayerInventory.Instance.LoadFromDisk();

        // Login panelini kapat (istersen)
        gameObject.SetActive(false);
    }
}
