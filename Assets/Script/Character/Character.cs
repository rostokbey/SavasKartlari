using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]

public class Character : NetworkBehaviour
{
    [Header("Stats")]
    public CardData cardData;
    public float maxHp;
    public float hp;          // <- ekranındaki “currentHealth” yerine basit hp
    public float defense = 0; // <- yoktu, eklendi
    private HealthSystem health;

    [Header("State")]
    public bool isMyTurn;

    [Header("Optional")]
    public Animator animator;

    // BattleManager -> ch.Setup(card) çağırıyor
    public void Setup(CardData data)
    {
        cardData = data;
        maxHp = data.baseHP;
        hp = maxHp;
        defense = 0f;

        name = data.cardName;
    }

    public void SetTurn(bool turn)
    {
        isMyTurn = turn;
        // (İstersen highlight/animasyon koy)
        // Debug.Log($"Sıra bende mi? {isMyTurn}");
    }

    // === BASİT SALDIRI ===
    [ServerRpc(RequireOwnership = false)]
    public void AttackServerRpc(NetworkObjectReference targetRef)
    {
        if (!isMyTurn) return;

        if (targetRef.TryGet(out var targetObj))
        {
            var targetChar = targetObj.GetComponent<Character>();
            if (targetChar != null)
            {
                float dmg = Mathf.Max(1f, cardData.baseDamage); // en az 1 hasar
                targetChar.ApplyDamage(dmg);
                // Debug.Log($"{cardData.cardName} -> {targetChar.cardData.cardName} : {dmg}");
            }
        }

        BattleManager.Instance.EndTurn();
    }

    



    // === HASAR / ŞİFA ===
    public void ApplyDamage(float damage)
    {
        float actual = Mathf.Max(0f, damage - defense);
        hp = Mathf.Max(0f, hp - actual);

        // Tüm client’larda popup göster
        SpawnDamagePopupClientRpc(Mathf.RoundToInt(actual), false);

        if (hp <= 0f) Die();
    }

    public void Heal(float amount)
    {
        float before = hp;
        hp = Mathf.Min(maxHp, hp + amount);
        float healed = hp - before;

        if (healed > 0.01f)
            SpawnDamagePopupClientRpc(Mathf.RoundToInt(healed), true);
    }

    [ClientRpc]
    void SpawnDamagePopupClientRpc(int amount, bool isHeal)
    {
        // popup pozisyonu biraz yukarıda
        var pos = transform.position + Vector3.up * 1.5f;
        DamagePopup.Create(pos, amount, isCritical: false, isHeal: isHeal);
    }

    void Die()
    {
        // İstersen ölüm animasyonu tetikle
        // if (animator) animator.SetTrigger("Die");

        // Sunucu sahibi objeyi kapatsın
        if (IsServer && NetworkObject != null)
            NetworkObject.Despawn(true);
        else
            Destroy(gameObject);
    }
}
