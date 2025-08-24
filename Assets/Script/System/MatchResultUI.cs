// Dosya adı: MatchResultUI.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchResultUI : MonoBehaviour
{
    [Header("Top Area")]
    public TextMeshProUGUI titleText;
    public Button okButton;

    [Header("List")]
    public Transform contentParent;      // ScrollView/Viewport/Content
    public GameObject rowPrefab;         // MatchResultRowUI bulunan prefab

    // Ekranı aç: kazandı mı, hangi kartlar ne kadar değişti
    public void Show(bool isWinner, List<CardXpDelta> deltas)
    {
        gameObject.SetActive(true);

        if (titleText)
            titleText.text = isWinner ? "Zafer! ✨" : "Maç Bitti";

        // Temizle
        foreach (Transform t in contentParent) Destroy(t.gameObject);

        // Satırları oluştur
        foreach (var d in deltas)
        {
            var go = Instantiate(rowPrefab, contentParent);
            var row = go.GetComponent<MatchResultRowUI>();
            if (row) row.Setup(d);
        }

        if (okButton)
        {
            okButton.onClick.RemoveAllListeners();
            okButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}

public class MatchResultRowUI : MonoBehaviour
{
    [Header("Refs")]
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public Slider xpBar;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI deltaText;

    CardXpDelta data;

    public void Setup(CardXpDelta d)
    {
        data = d;

        // İsim
        if (nameText) nameText.text = d.card?.cardName ?? "(?)";

        // İkon (Resources’tan çözmeye çalış)
        if (icon)
        {
            var sp = CardArtResolver.GetSprite(d.card, QRDataManager.Instance?.defaultListSprite);
            icon.sprite = sp;
            icon.enabled = (sp != null);
        }

        // Level ve bar
        if (levelText)
            levelText.text = $"Lv {d.oldLevel} → {d.newLevel}";

        // Bar max & value
        int barMax = CardLevelSystem.Instance
            ? CardLevelSystem.Instance.XpForNextLevel(Mathf.Max(1, d.newLevel))
            : Mathf.Max(1, d.newXp);

        if (xpBar)
        {
            xpBar.minValue = 0;
            xpBar.maxValue = barMax;
            xpBar.value = Mathf.Clamp(d.oldXp, 0, xpBar.maxValue);
        }

        if (xpText)
            xpText.text = $"{d.oldXp} / {barMax}";

        // Delta yazısı
        int net = (d.newLevel > d.oldLevel)
            ? (d.newXp + 100000) // sadece görsel vurgu için; net XP hesaplama senin sisteminde var
            : (d.newXp - d.oldXp);

        if (deltaText)
            deltaText.text = (d.newLevel > d.oldLevel)
                ? $"Seviye atladı! ({d.oldLevel}→{d.newLevel})"
                : (net >= 0 ? $"+{net} XP" : $"{net} XP");

        // Basit animasyon: eski seviyeden yeni seviyeye bar ilerlesin
        if (gameObject.activeInHierarchy)
            StartCoroutine(AnimateXp());
    }

    IEnumerator AnimateXp()
    {
        if (xpBar == null || CardLevelSystem.Instance == null) yield break;

        int curLevel = data.oldLevel;
        int curXp = data.oldXp;

        // Seviyeler arası geçiş
        while (curLevel < data.newLevel)
        {
            int need = CardLevelSystem.Instance.XpForNextLevel(curLevel);
            yield return LerpBar(curXp, need, 0.6f);
            // bir sonraki seviyeye sıfırdan başla
            xpBar.value = 0;
            curLevel++;
            curXp = 0;
        }

        // Son seviyedeki yeni XP’ye ilerle
        int maxAtFinal = CardLevelSystem.Instance.XpForNextLevel(Mathf.Max(1, data.newLevel));
        xpBar.maxValue = maxAtFinal;
        yield return LerpBar(curXp, Mathf.Clamp(data.newXp, 0, maxAtFinal), 0.6f);

        if (xpText) xpText.text = $"{data.newXp} / {maxAtFinal}";
    }

    IEnumerator LerpBar(float from, float to, float dur)
    {
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0, 1, t / dur);
            xpBar.value = Mathf.Lerp(from, to, k);
            yield return null;
        }
        xpBar.value = to;
    }
}
