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
        Debug.Log("Normal saldýrý yapýldý!");
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
        var card = GetComponent<PlayerInventory>()?.GetActiveCard();
        var inventory = GetComponent<PlayerInventory>();

        if (card == null || !inventory.CanUseSkill()) return;

        Debug.Log("Skill kullanýldý: " + card.ability);
        BattleManager.Instance.SendSkillAttackServerRpc(card.ability);
        inventory.ResetSkillCooldown();
        PerformSkillAttackClientRpc(card.ability);
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
