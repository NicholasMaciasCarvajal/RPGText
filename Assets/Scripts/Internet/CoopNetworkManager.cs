using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
// using static UnityEditor.Rendering.CoreEditorDrawer<TData>;

public class CoopNetworkManager : NetworkBehaviour
{
    public static CoopNetworkManager Instance;

    public List<NetworkPlayer> connectedPlayers = new();

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            enabled = false; // solo host lo usa
    }

    public void RegisterPlayer(NetworkPlayer player)
    {
        if (!IsServer) return;
        connectedPlayers.Add(player);
    }

    public void EndPlayerTurn(NetworkPlayer player)
    {
        if (!IsServer) return;

        Debug.Log($"Jugador terminó turno: {player.OwnerClientId}");
        GameManager.Instance.turnManager.EndTurnServer();
    }
}

public class TurnEndRequester : NetworkBehaviour
{
    [ServerRpc]
    public void RequestEndTurnServerRpc()
    {
        GameManager.Instance.turnManager.EndTurnServer();
    }
}