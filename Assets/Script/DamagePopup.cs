using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [Header("Refs")]
    public TMP_Text text;                 // Prefab i�indeki TMP_Text (atanmazsa Awake'te bulunur)

    [Header("Anim")]
    public float life = 0.8f;             // Ekranda kalma s�resi
    public Vector3 drift = new Vector3(0f, 1.5f, 0f); // Yukar� kayma y�n�
    public float speed = 2f;              // Kayma h�z�

    void Awake()
    {
        if (!text) text = GetComponentInChildren<TMP_Text>();
    }

    // --- Basit kullan�m: sadece say� g�ster ---
    public void Setup(int amount)
    {
        if (!text) text = GetComponentInChildren<TMP_Text>();

        text.text = amount.ToString();
        text.color = Color.white;         // D�z hasar rengi
        transform.localScale = Vector3.one * 0.01f; // 3D sahnede okunakl� �l�ek
    }

    // --- �ste�e ba�l�: eski �a�r�lar bo�a d��mesin diye �ok parametreli overloada izin veriyoruz ---
    public void Setup(int amount, bool isCritical, bool isHeal, bool isShieldGain)
    {
        Setup(amount);                    // Temel yaz�y� ayarla
        if (isHeal) text.color = Color.green;
        else if (isCritical) text.color = Color.yellow;
        else text.color = Color.white;
        // isShieldGain i�in �zel efekt eklemek istersen burada rengi/metin bi�imini de�i�tirebilirsin.
    }

    void Update()
    {
        // Hafif yukar� do�ru s�z�lme
        transform.position += drift * speed * Time.deltaTime;

        // �m�r
        life -= Time.deltaTime;
        if (life <= 0f) Destroy(gameObject);
    }

    // --- Opsiyonel factory: Script'ten rahat �a��rmak i�in ---
    // Resources/Prefabs/FX/DamagePopup yolunda prefab varsa bunu kullan�r.
    public static DamagePopup Create(Vector3 worldPos, int amount)
    {
        var prefab = Resources.Load<GameObject>("Prefabs/FX/DamagePopup");
        if (!prefab)
        {
            Debug.LogWarning("DamagePopup prefab bulunamad�: Resources/Prefabs/FX/DamagePopup");
            return null;
        }

        var go = Instantiate(prefab, worldPos, Quaternion.identity);
        var dp = go.GetComponent<DamagePopup>();
        dp.Setup(amount);
        return dp;
    }
}
