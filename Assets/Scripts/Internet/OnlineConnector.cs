using Unity.Netcode;
using UnityEngine;

public class OnlineConnector : MonoBehaviour
{
    public void StartAsHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("Host iniciado");
    }

    public void StartAsClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Cliente conectado");
    }

    public void Shutdown()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
