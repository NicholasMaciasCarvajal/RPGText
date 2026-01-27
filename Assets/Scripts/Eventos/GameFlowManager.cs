using Unity.Netcode;
using UnityEngine;
using static GameManager;

public enum GameState
{
    Narrative,
    Combat,
    Loot,
    Progression
}

public class GameFlowManager : NetworkBehaviour
{
    public static GameFlowManager Instance;

    public GameState currentState;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [Header("Pools de eventos")]
    public CombatEvent[] possibleCombats;
    public GameEvent[] possibleLootEvents;
    public NarrativeNode[] possibleNarratives;

    public CombatEvent GetRandomCombatEvent()
    {
        return possibleCombats[Random.Range(0, possibleCombats.Length)];
    }

    public GameEvent GetRandomLootEvent()
    {
        return possibleLootEvents[Random.Range(0, possibleLootEvents.Length)];
    }

    public NarrativeNode GetRandomNarrativeNode()
    {
        return possibleNarratives[Random.Range(0, possibleNarratives.Length)];
    }


    // =================== NARRATIVA ===================

    public void EnterNarrative(NarrativeNode node)
    {
        if (!IsServer) return;

        currentState = GameState.Narrative;

        NarrativeHUDController.Instance.Show();
        NarrativeManager.Instance.StartNarrative(node);
    }

    public void EndNarrative()
    {
        if (!IsServer) return;

        currentState = GameState.Progression;

        NarrativeHUDController.Instance.Hide();
        ProgressionManager.Instance.ContinueAfterEvent();
    }

    // =================== COMBATE ===================

    public void EnterCombat(CombatEvent combatEvent)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        Debug.Log("[FLOW] Entrando en combate");

        if (NarrativeHUDController.Instance == null)
            Debug.LogError("[FLOW] NarrativeHUDController.Instance es NULL");

        if (NetworkBattleManager.Instance == null)
            Debug.LogError("[FLOW] NetworkBattleManager.Instance es NULL");

        if (NarrativeHUDController.Instance != null)
            NarrativeHUDController.Instance.Hide();

        currentState = GameState.Combat;

        if (NetworkBattleManager.Instance != null)
            NetworkBattleManager.Instance.StartBattle(combatEvent);
    }


    public void EndCombatVictory()
    {
        if (!IsServer) return;

        Debug.Log("[FLOW] Combate ganado");

        currentState = GameState.Progression;
        ProgressionManager.Instance.ContinueAfterEvent();
    }

    /*
    internal void EnterRandomCombat(CombatEvent combatEvent)
    {
        NarrativeHUDController.Instance.Hide();

        currentState = GameState.Combat;

        Debug.Log("[FLOW] Entrando en combate");

        NetworkBattleManager.Instance.StartBattle(combatEvent);
    }
    */

    public void EndCombatDefeat()
    {
        if (!IsServer) return;

        Debug.Log("[FLOW] Combate perdido");

        // Game over o nodo especial
    }

    // =================== LOOT (placeholder) ===================

    public void EnterLoot()
    {
        if (!IsServer) return;

        currentState = GameState.Loot;

        Debug.Log("[FLOW] Entrando en loot");

        // temporal: pasar a narrativa
        EnterNarrative(GetRandomNarrativeNode());
    }
}
