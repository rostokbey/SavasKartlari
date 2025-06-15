using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;

public class LobbyUIManager : MonoBehaviour
{
    public Button hostButton;
    public Button joinButton;
    public InputField joinCodeInput;

    private async void Start()
    {
        await Unity.Services.Core.UnityServices.InitializeAsync();

        if (joinCodeInput == null)
        {
            Debug.LogError(" Join Code Input atanmamış!");
        }

        hostButton.onClick.AddListener(HostGame);
        joinButton.onClick.AddListener(JoinGame);
    }

    public async void HostGame()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(new RelayServerData(allocation, "dtls"));

        NetworkManager.Singleton.StartHost();
        Debug.Log("Host started with code: " + joinCode);

        // Doğru enum gönderimi:
        FindObjectOfType<SceneUIController>()?.ShowOnly(SceneUIController.TargetPanel.DeckBuilder);
    }

    public async void JoinGame()
    {
        string joinCode = joinCodeInput.text;

        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(new RelayServerData(allocation, "dtls"));

        NetworkManager.Singleton.StartClient();
        Debug.Log("Joined game with code: " + joinCode);

        // Doğru enum gönderimi:
        FindObjectOfType<SceneUIController>()?.ShowOnly(SceneUIController.TargetPanel.DeckBuilder);
    }
}
