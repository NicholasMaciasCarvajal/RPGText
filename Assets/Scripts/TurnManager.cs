using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TurnManager : NetworkBehaviour
{
    public enum TurnOwner
    {
        Player1AndPlayer2,
        Enemies
    }


    [Header("Turn Order")]
    public List<TurnOwner> turnOrder = new List<TurnOwner>();

    private NetworkVariable<int> currentTurnIndex =
        new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public TurnOwner CurrentTurn => turnOrder[currentTurnIndex.Value];

    private void Start()
    {
        if (IsServer)
        {
            InitializeTurnsServer();
            StartTurnServer();
        }

        currentTurnIndex.OnValueChanged += OnTurnChanged;
    }

    private void OnDestroy()
    {
        currentTurnIndex.OnValueChanged -= OnTurnChanged;
    }

    // ---------------- SERVER LOGIC ----------------

    private void InitializeTurnsServer()
    {
        turnOrder.Clear();

        turnOrder.Add(TurnOwner.Player1AndPlayer2);
        turnOrder.Add(TurnOwner.Enemies);

        currentTurnIndex.Value = 0;
    }


    private void StartTurnServer()
    {
        Debug.Log($"[SERVER] Inicia turno: {CurrentTurn}");

        var battle = FindFirstObjectByType<NetworkBattleManager>();

        switch (CurrentTurn)
        {
            case TurnOwner.Player1AndPlayer2:
                EnablePlayersSelectionClientRpc();
                break;


            case TurnOwner.Enemies:
                battle.ResolveEnemiesPhaseServer();
                break;
        }
    }


    public void EndTurnServer()
    {
        if (!IsServer) return;

        currentTurnIndex.Value++;

        if (currentTurnIndex.Value >= turnOrder.Count)
            currentTurnIndex.Value = 0;

        StartTurnServer();
    }

    private void HandleEnemyTurnServer()
    {
        Debug.Log("[SERVER] Turno de enemigos");

        // aquí luego conectarás IA
        Invoke(nameof(FinishEnemyTurnServer), 1.0f);
    }

    private void FinishEnemyTurnServer()
    {
        EndTurnServer();
    }

    // ---------------- CLIENT FEEDBACK ----------------

    private void OnTurnChanged(int prev, int next)
    {
        Debug.Log($"[CLIENT] Cambio de turno a {CurrentTurn}");
    }

    [ClientRpc]
    private void EnablePlayersSelectionClientRpc()
    {
        GameManager.Instance.player1.GetComponent<PlayerTurnController>()?.ResetTurnInput();
        GameManager.Instance.player2.GetComponent<PlayerTurnController>()?.ResetTurnInput();
    }


}
