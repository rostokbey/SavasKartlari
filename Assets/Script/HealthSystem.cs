using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Collections;

public class HealthSystem : NetworkBehaviour
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private GameObject damagePopupPrefab;

    private NetworkVariable<int> currentHP = new NetworkVariable<int>(100);
    private int maxHP = 100;
    private int shield = 0;

    public override void OnNetworkSpawn()
    {
        currentHP.OnValueChanged += OnHealthChanged;

        if (IsOwner && hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP.Value;
        }
    }

    private void OnHealthChanged(int oldVal, int newVal)
    {
        if (IsOwner && hpSlider != null)
        {
            hpSlider.value = newVal;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        int finalDamage = Mathf.Max(0, damage - shield);
        shield = Mathf.Max(0, shield - damage);
        currentHP.Value = Mathf.Max(0, currentHP.Value - finalDamage);

        Debug.Log($"{OwnerClientId} hasar aldı: {finalDamage}, kalan HP: {currentHP.Value}");

        ShowDamageClientRpc(finalDamage);

        if (currentHP.Value <= 0)
        {
            Debug.Log($"{OwnerClientId} öldü!");
        }
    }

    [ClientRpc]
    private void ShowDamageClientRpc(int damage)
    {
        if (damagePopupPrefab != null)
        {
            var popup = Instantiate(damagePopupPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            popup.GetComponent<DamagePopup>().Setup(damage, false, false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void HealServerRpc(int amount)
    {
        currentHP.Value = Mathf.Min(currentHP.Value + amount, maxHP);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddShieldServerRpc(int amount)
    {
        shield += amount;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageOverTimeServerRpc(int damagePerTick, int ticks)
    {
        StartCoroutine(DamageOverTimeCoroutine(damagePerTick, ticks));
    }

    private IEnumerator DamageOverTimeCoroutine(int damage, int times)
    {
        for (int i = 0; i < times; i++)
        {
            TakeDamageServerRpc(damage);
            yield return new WaitForSeconds(1.5f);
        }
    }
}
