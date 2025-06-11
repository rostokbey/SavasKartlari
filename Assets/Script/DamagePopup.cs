using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TextMeshPro textMesh;
    public float floatSpeed = 2f;
    public float destroyTime = 1f;

    public void Setup(int damage)
    {
        textMesh.text = damage.ToString();
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }
}
