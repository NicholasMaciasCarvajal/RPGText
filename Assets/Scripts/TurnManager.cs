using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using static NetworkBattleManager;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager Instance;

    public enum TurnOwner
    {
        Player1 = 0,
        Player2 = 1,
        Enemies = 2
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    private NetworkVariable<int> currentTurnIndex =
        new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public TurnOwner CurrentTurnO => (TurnOwner)currentTurnIndex.Value;

    private void Start()
    {
        currentTurnIndex.OnValueChanged += OnTurnChanged;
    }


    private void OnDestroy()
    {
        currentTurnIndex.OnValueChanged -= OnTurnChanged;
    }

    // ---------------- SERVER LOGIC ----------------

    /*
    public void InitializeTurnsServer()
    {
        turnOrder.Clear();

        if (GameManager.Instance.player1 != null)
            turnOrder.Add(TurnOwner.Player1);

        if (GameManager.Instance.player2 != null)
            turnOrder.Add(TurnOwner.Player2);

        turnOrder.Add(TurnOwner.Enemies);

        currentTurnIndex.Value = 0;
    }
    */

    public void StartBattleTurnsServer()
    {
        if (!IsServer) return;

        Debug.Log("[TURN] Inicializando sistema de turnos");

        //InitializeTurnsServer();
        StartTurnServer();
    }

    public void BeginBattleServer()
    {
        if (!IsServer) return;

        currentTurnIndex.Value = 0;
        StartTurnServer();
    }



    public void StartTurnServer()
    {
        Debug.Log($"[SERVER] Inicia turno: {CurrentTurnO}");

        var battle = FindFirstObjectByType<NetworkBattleManager>();

        switch (CurrentTurnO)
        {
            case TurnOwner.Player1:
            case TurnOwner.Player2:
                EnablePlayersSelectionClientRpc(CurrentTurnO);
                break;


            case TurnOwner.Enemies:

                ProcessTurnStartServer();

                // aplicar OnTurnStart a enemigos
                foreach (var enemy in battle.enemies)
                {
                    if (enemy != null && enemy.isAlive)
                        enemy.OnTurnStart();
                }

                battle.ResolveEnemiesPhaseServer();
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResetToPlayersTurnServerRpc()
    {
        if (!IsServer) return;

        Debug.Log("[TURN] Reiniciando fase de jugadores");

        currentTurnIndex.Value = 0;

        StartTurnServer();
    }



    private void ProcessTurnStartServer()
    {
        foreach (var player in new[] { GameManager.Instance.player1, GameManager.Instance.player2 })
        {
            if (player != null && player.isAlive)
                player.OnTurnStart();
        }

        foreach (var enemy in FindObjectsByType<EnemyCharacter>(FindObjectsSortMode.None))
        {
            if (enemy != null && enemy.isAlive)
                enemy.OnTurnStart();
        }
    }


    public void EndTurnServer()
    {
        if (!IsServer) return;

        // aplicar OnTurnEnd al grupo actual
        var battle = FindFirstObjectByType<NetworkBattleManager>();

        switch (CurrentTurnO)
        {
            case TurnOwner.Player1:
                GameManager.Instance.player1?.OnTurnEnd();
                break;
            case TurnOwner.Player2:
                GameManager.Instance.player2?.OnTurnEnd();
                break;
            case TurnOwner.Enemies:
                foreach (var enemy in battle.enemies)
                {
                    if (enemy != null && enemy.isAlive)
                        enemy.OnTurnEnd();
                }
                break;
        }

        currentTurnIndex.Value++;

        if (currentTurnIndex.Value > 2)
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
        var newTurn = (TurnOwner)next;

        Debug.Log($"[CLIENT] Cambio de turno a {newTurn}");

        //  Primero: apagar UI de todos
        DisablePlayersSelectionClientRpc();

        //  Luego: activar solo al jugador correcto
        EnablePlayersSelectionClientRpc(newTurn);
    }

    [ClientRpc]
    private void EnablePlayersSelectionClientRpc(TurnOwner currentTurn)
    {
        if (NetworkManager.Singleton.LocalClient.PlayerObject == null)
            return;

        var localController = NetworkManager.Singleton.LocalClient.PlayerObject
            .GetComponent<PlayerTurnController>();

        var localPlayer = localController.GetComponent<PlayerCharacter>();

        ulong localClientId = NetworkManager.Singleton.LocalClientId;

        bool isMyTurn = false;

        if (currentTurn == TurnOwner.Player1 &&
            localClientId == GameManager.Instance.player1ClientId.Value)
        {
            isMyTurn = true;
        }
        else if (currentTurn == TurnOwner.Player2 &&
                 localClientId == GameManager.Instance.player2ClientId.Value)
        {
            isMyTurn = true;
        }

        Debug.Log($"[CLIENT] Checando turno. Local={localClientId} Turno={currentTurn} ¿Es mío? {isMyTurn}");

        if (!isMyTurn)
            return;

        // AHORA SÍ ES MI TURNO

        CombatHUDController.Instance.SetTurnText("Tu turno");

        localController.ResetTurnInput();

        CombatHUDController.Instance.ShowAbilities(
            localController,
            localPlayer.abilities
        );

        Debug.Log("[CLIENT] Es mi turno, UI activada");
    }



    [ClientRpc]
    private void DisablePlayersSelectionClientRpc()
    {
        // Ocultar habilidades en TODOS los clientes
        CombatHUDController.Instance.HideAbilities();

        // Bloquear input local si existe
        if (NetworkManager.Singleton.LocalClient.PlayerObject != null)
        {
            var ctrl = NetworkManager.Singleton.LocalClient.PlayerObject
                .GetComponent<PlayerTurnController>();

            if (ctrl != null)
                ctrl.GetComponent<PlayerCharacter>().EnableInput(false);
        }

        Debug.Log("[CLIENT] UI desactivada, esperando turno...");
    }


    public CharacterBase CurrentCharacter
    {
        get
        {
            switch (CurrentTurnO)
            {
                case TurnOwner.Player1:
                    return GameManager.Instance.player1;

                case TurnOwner.Player2:
                    return GameManager.Instance.player2;

                case TurnOwner.Enemies:
                    var battle = FindFirstObjectByType<NetworkBattleManager>();
                    foreach (var enemy in battle.enemies)
                        if (enemy != null && enemy.isAlive)
                            return enemy;
                    return null;
            }

            return null;
        }
    }


}
