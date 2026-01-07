using UnityEngine;

public class CoopManager : MonoBehaviour
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

    private void Start()
    {
        StartNextTurn();
    }

    public void StartNextTurn()
    {
        if (allowJointActions)
        {
            player1.EnableInput(true);
            player2.EnableInput(true);
        }
        else
        {
            // Solo el jugador cuyo turno es activo puede actuar
            TurnManager.TurnOwner current = GameManager.Instance.turnManager.CurrentTurn;

            switch (current)
            {
                case TurnManager.TurnOwner.Player1:
                    player1.EnableInput(true);
                    player2.EnableInput(false);
                    break;

                case TurnManager.TurnOwner.Player2:
                    player1.EnableInput(false);
                    player2.EnableInput(true);
                    break;

                case TurnManager.TurnOwner.Enemies:
                    player1.EnableInput(false);
                    player2.EnableInput(false);
                    break;
            }
        }
    }

    public void EndCurrentPlayerTurn(PlayerCharacter player)
    {
        player.EnableInput(false);

        if (!allowJointActions)
        {
            // Avanzar turno en TurnManager
            GameManager.Instance.turnManager.EndTurn();
            // Activar input del siguiente jugador
            StartNextTurn();
        }
        else
        {
            // Si ambos pueden actuar, se controla por separado
            Debug.Log($"{player.name} terminó su acción (modo conjunto).");
        }
    }

    /*
    [PunRPC]
    void RPC_UseAbility(int abilityIndex, int targetId)
    {
        // Ejecuta la habilidad en todos los clientes
        Ability ability = player.abilities[abilityIndex];
        CharacterBase target = BattleManager.Instance.GetCharacterById(targetId);
        AbilityExecutor.ExecuteAbility(player, target, ability);
    }
    Para multijugador Online concepto para photon
    */
}
