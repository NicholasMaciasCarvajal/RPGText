using UnityEngine;
using static GameManager;

public enum GameState
{
    Narrative,
    Combat,
    Loot
}


public class GameFlowManager : MonoBehaviour
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
        NarrativeHUDController.Instance.Show();
        GameManager.Instance.SetState(GameManager.GameState.Narrative);
        NarrativeManager.Instance.StartNarrative(node);
    }

    public void EndNarrative()
    {
        NarrativeHUDController.Instance.Hide();
        GameManager.Instance.SetState(GameManager.GameState.Narrative);
        ProgressionManager.Instance.ContinueAfterEvent();
    }

    // =================== COMBATE ===================

    public void EnterCombat(CombatEvent combatEvent)
    {
        NarrativeHUDController.Instance.Hide();

        currentState = GameState.Combat;

        Debug.Log("[FLOW] Entrando en combate");

        NetworkBattleManager.Instance.StartBattle(combatEvent);
    }

    public void EndCombatVictory()
    {
        Debug.Log("[FLOW] Combate ganado");

        ProgressionManager.Instance.ContinueAfterEvent();
    }

    internal void EnterRandomCombat(CombatEvent combatEvent)
    {
        NarrativeHUDController.Instance.Hide();

        currentState = GameState.Combat;

        Debug.Log("[FLOW] Entrando en combate");

        NetworkBattleManager.Instance.StartBattle(combatEvent);
    }

    public void EndCombatDefeat()
    {
        Debug.Log("[FLOW] Combate perdido");

        // Game over o nodo especial
    }

    // =================== LOOT (placeholder) ===================

    public void EnterLoot()
    {
        currentState = GameState.Loot;

        Debug.Log("[FLOW] Entrando en loot");

        // luego implementamos loot UI
        EnterNarrative(GetRandomNarrativeNode());
    }
}
