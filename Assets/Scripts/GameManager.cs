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
    public NetworkVariable<ulong> player1ClientId = new NetworkVariable<ulong>();
    public NetworkVariable<ulong> player2ClientId = new NetworkVariable<ulong>();

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

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private System.Collections.IEnumerator AssignPlayerAfterSpawn(ulong clientId)
    {
        // Esperar a que el PlayerObject exista realmente
        yield return new WaitUntil(() =>
            NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId) &&
            NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject != null
        );

        var playerObj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        var player = playerObj.GetComponent<PlayerCharacter>();

        if (player == null)
        {
            Debug.LogError("[GAME] PlayerObject no tiene PlayerCharacter");
            yield break;
        }

        if (player1 == null)
        {
            player1 = player;
            player1ClientId.Value = clientId;
            Debug.Log($"[GAME] Player 1 asignado (Client {clientId})");
        }
        else if (player2 == null)
        {
            player2 = player;
            player2ClientId.Value = clientId;
            Debug.Log($"[GAME] Player 2 asignado (Client {clientId})");
        }
        else
        {
            Debug.LogWarning("[GAME] Ya hay 2 jugadores registrados.");
            yield break;
        }

        // Si ya están los dos, iniciar tutorial
        if (player1 != null && player2 != null)
        {
            StartTutorialCombat();
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        StartCoroutine(AssignPlayerAfterSpawn(clientId));
    }

    private void StartTutorialCombat()
    {
        if (!IsServer) return;

        Debug.Log("[GAME] Iniciando combate tutorial");

        var eventResolver = FindFirstObjectByType<EventResolver>();

        if (eventResolver != null && tutorialCombat != null)
        {
            eventResolver.ResolveEvent(tutorialCombat);
        }
    }

    public void RecargarEnergiaP1()
    {
        Debug.Log("Recargando 3 de Energia");
        player1.currentEnergy = player1.currentEnergy + 3;
    }

    public void RecargarEnergiaP2()
    {
        Debug.Log("Recargando 3 de Energia");
        player2.currentEnergy = player2.currentEnergy + 3;
    }
}
