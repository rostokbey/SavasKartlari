using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TMP_Text text;

    float life = 0.8f;
    Vector3 drift = new Vector3(0f, 1.5f, 0f);
    float speed = 2f;

    // Resources/Prefabs/DamagePopup altýnda prefab arar
    public static DamagePopup Create(Vector3 worldPos, int value, bool isCritical, bool isHeal)
    {
        var prefab = Resources.Load<GameObject>("Prefabs/DamagePopup");
        var go = Instantiate(prefab, worldPos, Quaternion.identity);
        var dp = go.GetComponent<DamagePopup>();
        dp.Setup(value, isCritical, isHeal);
        return dp;
    }

    public void Setup(int value, bool isCritical, bool isHeal)
    {
        if (!text) text = GetComponentInChildren<TMP_Text>();
        text.text = (isHeal ? "+" : "-") + value;

        if (isHeal) text.color = Color.green;
        else if (isCritical) text.color = Color.yellow;
        else text.color = Color.white;

        transform.position += new Vector3(Random.Range(-0.2f, 0.2f), 0f, 0f);
        transform.localScale = Vector3.one * 0.01f; // 3D TextMeshPro ise mobilde okunur
    }

    void Update()
    {
        transform.position += drift * (speed * Time.deltaTime);
        life -= Time.deltaTime;
        if (life <= 0f) Destroy(gameObject);
    }
}
