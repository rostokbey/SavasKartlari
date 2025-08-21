using UnityEngine;
using Unity.Netcode;

public class HealthSystem : NetworkBehaviour
{
    [Header("HP & Shield")]
    public int maxHP = 100;
    public NetworkVariable<int> currentHP = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public int shield = 0;

    [Header("FX")]
    [Tooltip("Resources/Prefabs/FX/DamagePopup.prefab gibi bir prefab")]
    public GameObject damagePopupPrefab;

    void Awake()
    {
        // Editor’de test ederken boş gelirse güvenlik ağı:
        if (maxHP <= 0) maxHP = 100;
    }

    public override void OnNetworkSpawn()
    {
        // Sunucu kişinin canını başlatır
        if (IsServer)
        {
            if (currentHP.Value <= 0) currentHP.Value = maxHP;
        }
    }

    // ====== Server API ======
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        int beforeShield = shield;

        // Kalkan önce hasarı emer
        int shieldAbsorb = Mathf.Min(shield, damage);
        shield -= shieldAbsorb;

        int remaining = Mathf.Max(0, damage - shieldAbsorb);
        currentHP.Value = Mathf.Max(0, currentHP.Value - remaining);

        Debug.Log($"[HS] Hasar {damage} (kalkan {beforeShield}->{shield}) kalan HP: {currentHP.Value}");

        ShowDamageClientRpc(remaining, false, false, false); // crit=false, heal=false, shield=false
        if (currentHP.Value <= 0)
            Die();
    }

    [ServerRpc(RequireOwnership = false)]
    public void HealServerRpc(int amount)
    {
        if (amount <= 0) return;
        int old = currentHP.Value;
        currentHP.Value = Mathf.Min(maxHP, currentHP.Value + amount);
        int gained = currentHP.Value - old;

        if (gained > 0)
            ShowDamageClientRpc(gained, false, true, false); // heal
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddShieldServerRpc(int amount)
    {
        if (amount <= 0) return;
        shield += amount;
        ShowDamageClientRpc(amount, false, false, true); // shield gain
    }

    // ====== Client FX ======
    [ClientRpc]
    private void ShowDamageClientRpc(int amount, bool isCritical, bool isHeal, bool isShieldGain)
    {
        if (damagePopupPrefab == null) return;

        var pos = transform.position + Vector3.up * 1.5f;
        var popup = Object.Instantiate(damagePopupPrefab, pos, Quaternion.identity);

        var dp = popup.GetComponent<DamagePopup>();
        if (dp != null)
        {
            // Senin DamagePopup imzan:
            //dp.Setup(amount);

            // Eğer sende sadece tek parametreli varsa, şunu kullan:
             dp.Setup(amount);
        }
    }

    // ====== Utils ======
    private void Die()
    {
        Debug.Log($"[HS] {OwnerClientId} öldü.");
        // Basitçe yok et; istersen anim/olay tetikle
        var netObj = GetComponent<NetworkObject>();
        if (IsServer)
        {
            if (netObj) netObj.Despawn(true);
            else Destroy(gameObject);
        }
    }
}
