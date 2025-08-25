using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    private Animator animator;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            animator = GetComponent<Animator>();
    }

    [ServerRpc]
    public void PerformAttackServerRpc()
    {
        Debug.Log("Normal sald�r� yap�ld�!");
        BattleManager.Instance.SendAttackServerRpc();
        PerformAttackClientRpc();
    }

    [ClientRpc]
    private void PerformAttackClientRpc()
    {
        if (animator != null)
            animator.SetTrigger("Attack");
    }

    [ServerRpc]
    public void PerformSkillAttackServerRpc()
    {
        var inventory = GetComponent<PlayerInventory>();
        var card = inventory?.GetActiveCard();   // aktif kart� al

        if (card == null || !inventory.CanUseSkill(card))   // <-- parametre ver
            return;

        Debug.Log($"Skill kullan�ld�: {card.ability}");
        BattleManager.Instance.SendSkillAttackServerRpc(card.ability);

        inventory.ResetSkillCooldown(card);                 // <-- parametre ver
    }


    [ClientRpc]
    private void PerformSkillAttackClientRpc(string ability)
    {
        if (animator == null) return;

        switch (ability)
        {
            case "Fireball": animator.SetTrigger("Skill"); break;
            case "Poison": animator.SetTrigger("Poison"); break;
            case "Heal": animator.SetTrigger("Heal"); break;
            case "Shield": animator.SetTrigger("Shield"); break;
        }
    }
}
