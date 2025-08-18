using UnityEngine;
using Unity.Netcode;

public class Character : NetworkBehaviour
{
    public CardData cardData;
    public int hp;
    public bool isMyTurn = false;

    public void Setup(CardData data)
    {
        cardData = data ?? cardData;
        hp = Mathf.Max(0, cardData != null ? cardData.baseHP : 0);
    }

    public void SetTurn(bool turn)
    {
        isMyTurn = turn;
        if (IsLocalPlayer)
            Debug.Log("✅ Senin sıran mı? " + isMyTurn);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AttackServerRpc(NetworkObjectReference targetRef)
    {
        if (!isMyTurn) return;

        if (targetRef.TryGet(out var targetObj))
        {
            var targetChar = targetObj.GetComponent<Character>();
            targetChar.hp -= cardData.baseDamage;
            Debug.Log($"{cardData.cardName} saldırdı, {targetChar.cardData.cardName} yeni HP: {targetChar.hp}");

            BattleManager.Instance.EndTurn();
        }
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log($"[CHAR] Spawn: {cardData?.cardName ?? "NULL"} owner={OwnerClientId}");
    }

}
