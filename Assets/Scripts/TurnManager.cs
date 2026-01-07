using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public enum TurnOwner
    {
        Player1,
        Player2,
        Enemies
    }

    [Header("Turn Order")]
    public List<TurnOwner> turnOrder = new List<TurnOwner>();

    private int currentTurnIndex = 0;

    public TurnOwner CurrentTurn => turnOrder[currentTurnIndex];

    private void Start()
    {
        InitializeTurns();
    }

    public void InitializeTurns()
    {
        turnOrder.Clear();

        // Orden base (puedes hacerlo dinámico después)
        turnOrder.Add(TurnOwner.Player1);
        turnOrder.Add(TurnOwner.Player2);
        turnOrder.Add(TurnOwner.Enemies);

        currentTurnIndex = 0;
    }

    public void StartTurn()
    {
        Debug.Log($"Turno de: {CurrentTurn}");

        switch (CurrentTurn)
        {
            case TurnOwner.Player1:
                EnablePlayerInput(GameManager.Instance.player1);
                break;

            case TurnOwner.Player2:
                EnablePlayerInput(GameManager.Instance.player2);
                break;

            case TurnOwner.Enemies:
                StartEnemyTurn();
                break;
        }
    }

    public void EndTurn()
    {
        currentTurnIndex++;

        if (currentTurnIndex >= turnOrder.Count)
        {
            currentTurnIndex = 0;
        }

        StartTurn();
    }

    private void EnablePlayerInput(PlayerCharacter player)
    {
        player.EnableInput(true);
    }

    private void StartEnemyTurn()
    {
        // Aquí luego conectaremos la IA
        Debug.Log("Turno de los enemigos");

        // Simulación: enemigos actúan y terminan turno
        Invoke(nameof(FinishEnemyTurn), 1f);
    }

    private void FinishEnemyTurn()
    {
        EndTurn();
    }
}
