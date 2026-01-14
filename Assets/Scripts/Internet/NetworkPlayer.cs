using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    public PlayerCharacter linkedCharacter;   // referencia local a tu PlayerCharacter
    public PlayerTurnController turnController;

    private void Awake()
    {
        linkedCharacter = GetComponent<PlayerCharacter>();
        turnController = GetComponent<PlayerTurnController>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Debug.Log("Soy el dueño de este NetworkPlayer");
        }
    }
}
