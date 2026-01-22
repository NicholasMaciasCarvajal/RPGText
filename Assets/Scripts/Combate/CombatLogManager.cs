using Unity.Netcode;
using UnityEngine;

public class CombatLogManager : NetworkBehaviour
{
    public static CombatLogManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Llamado SOLO por el servidor
    public void LogServer(string message)
    {
        if (!IsServer) return;

        Debug.Log("[SERVER LOG] " + message);
        BroadcastLogClientRpc(message);
    }

    [ClientRpc]
    private void BroadcastLogClientRpc(string message)
    {
        Debug.Log("[COMBAT] " + message);

        // Aquí luego conectarás UI:
        // CombatUI.Instance.AddLine(message);
    }
}