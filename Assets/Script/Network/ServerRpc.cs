using UnityEngine;
using Unity.Netcode;

public class ServerRpcExample : NetworkBehaviour
{
    [ServerRpc]
    void MyServerRpc()
    {
        Debug.Log("ServerRpc �al��t�");
    }
}
