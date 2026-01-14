using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using System.Threading.Tasks;

public class NetworkConnectionManager : MonoBehaviour
{
    public static NetworkConnectionManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private async void Start()
    {
        await InitializeUnityServices();
    }

    private async Task InitializeUnityServices()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Signed in anonymously");
        }
    }

    // ------------------ HOST ------------------

    public async Task<string> CreateHostAsync(int maxPlayers = 2)
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);

        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(allocation.RelayServer.IpV4,
                                     (ushort)allocation.RelayServer.Port,
                                     allocation.AllocationIdBytes,
                                     allocation.Key,
                                     allocation.ConnectionData);

        NetworkManager.Singleton.StartHost();

        Debug.Log($"Host creado. Código de unión: {joinCode}");

        return joinCode;
    }

    // ------------------ CLIENT ------------------

    public async Task JoinByCodeAsync(string joinCode)
    {
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(allocation.RelayServer.IpV4,
                                     (ushort)allocation.RelayServer.Port,
                                     allocation.AllocationIdBytes,
                                     allocation.Key,
                                     allocation.ConnectionData,
                                     allocation.HostConnectionData);

        NetworkManager.Singleton.StartClient();

        Debug.Log("Cliente conectado mediante código");
    }
    public async void OnHostButton()
    {
        string code = await NetworkConnectionManager.Instance.CreateHostAsync();
        Debug.Log("Comparte este código: " + code);
    }

    public async void OnJoinButton(string codeInput)
    {
        await NetworkConnectionManager.Instance.JoinByCodeAsync(codeInput);
    }

    public TMPro.TMP_InputField codeField;

    public void JoinClicked()
    {
        OnJoinButton(codeField.text);
    }


}
