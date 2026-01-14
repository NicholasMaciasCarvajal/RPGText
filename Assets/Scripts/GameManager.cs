using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState
    {
        Narrative,
        Combat,
        Pause
    }

    [Header("Game State")]
    public GameState CurrentState;

    [Header("Player References")]
    public PlayerCharacter player1;
    public PlayerCharacter player2;

    [Header("Managers")]
    public TurnManager turnManager;
    public BattleManager battleManager;
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
}
