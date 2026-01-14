using Unity.Netcode;
using UnityEngine;

public class CoopManager : NetworkBehaviour
{
    public static CoopManager Instance;

    [Header("Players")]
    public PlayerCharacter player1;
    public PlayerCharacter player2;

    [Header("Settings")]
    public bool allowJointActions = false; // si true, ambos pueden actuar al mismo tiempo

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void EndCurrentPlayerTurn(PlayerCharacter player)
    {
        player.EnableInput(false);

        if (!allowJointActions)
        {
            // Avanzar turno en TurnManager
            GameManager.Instance.turnManager.EndTurnServer();
            // Activar input del siguiente jugador
        }
        else
        {
            // Si ambos pueden actuar, se controla por separado
            Debug.Log($"{player.name} terminó su acción (modo conjunto).");
        }
    }
}
