using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public enum GameState
    {
        Narrative,
        Combat,
        Pause
    }

    [Header("Tutorial")]
    public CombatEvent tutorialCombat;

    [Header("Game State")]
    public GameState CurrentState;

    [Header("Player References")]
    public PlayerCharacter player1;
    public PlayerCharacter player2;

    [Header("Managers")]
    public TurnManager turnManager;
    public NetworkBattleManager battleManager;
    public NarrativeManager narrativeManager;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetState(GameState.Narrative);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        AssignPlayers();
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Narrative:
                // narrativeManager?.StartNarrative();
                break;

            case GameState.Combat:
                // battleManager?.StartBattle();
                break;

            case GameState.Pause:
                Time.timeScale = 0f;
                break;
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        SetState(GameState.Narrative);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void AssignPlayers()
    {
        var players = FindObjectsOfType<PlayerCharacter>();

        if (players.Length >= 1)
            player1 = players[0];

        if (players.Length >= 2)
            player2 = players[1];

        Debug.Log("[GAME] Jugadores asignados");

        // si ya están los dos, iniciar tutorial
        if (player1 != null && player2 != null)
        {
            StartTutorialCombat();
        }
    }

    private void StartTutorialCombat()
    {
        Debug.Log("[GAME] Iniciando combate tutorial");

        var eventResolver = FindFirstObjectByType<EventResolver>();

        if (eventResolver != null && tutorialCombat != null)
        {
            eventResolver.ResolveEvent(tutorialCombat);
        }
    }
}
