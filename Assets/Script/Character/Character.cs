using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class Character : NetworkBehaviour
{
    [Header("Stats")]
    public CardData cardData;
    public float maxHp;
    public float hp;
    public float defense = 0f;

    private HealthSystem health;

    [Header("State")]
    public bool isMyTurn;

    [Header("Optional")]
    public Animator animator;

    // BattleManager -> ch.Setup(card) çağırıyor
    public void Setup(CardData data)
    {
        cardData = data;

        if (health == null) health = GetComponent<HealthSystem>();
        if (health != null)
        {
            health.maxHP = data.baseHP;
            // sunucu tarafında OnNetworkSpawn set ediyor, ama editor testte yardımcı olsun:
            if (IsServer)
            {
                health.currentHP.Value = data.baseHP;
                health.shield = 0;
            }
        }

        maxHp = data.baseHP;
        hp = maxHp;
        defense = 0f;
        name = data.cardName;
    }

    public void SetTurn(bool turn)
    {
        isMyTurn = turn;
        // İstersen highlight/animasyon
        // Debug.Log($"Sıra bende mi? {isMyTurn}");
    }

    // === BASİT SALDIRI (HealthSystem kullanır) ===
    [ServerRpc(RequireOwnership = false)]
    public void AttackServerRpc(NetworkObjectReference targetRef)
    {
        if (!isMyTurn) return;

        if (targetRef.TryGet(out var targetObj))
        {
            var targetHS = targetObj.GetComponent<HealthSystem>();
            if (targetHS != null)
            {
                int dmg = Mathf.Max(1, cardData.baseDamage); // basit örnek
                targetHS.TakeDamageServerRpc(dmg);
                Debug.Log($"[CHAR] {cardData.cardName} vurdu: {dmg}");
            }
        }

        BattleManager.Instance?.EndTurn();
    }
}
