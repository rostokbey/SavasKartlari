using UnityEngine;
using Unity.Netcode;

public class PerformSkillAttackServerRpc : NetworkBehaviour
{
    [ServerRpc]
    public void SendAttackServerRpc()
    {
        Debug.Log("Sald�r� server'a g�nderildi.");
    }
}
