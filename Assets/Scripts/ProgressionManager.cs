using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance;

    [Header("Nodo inicial")]
    public ProgressionNode startingNode;

    private ProgressionNode currentNode;
    private EventResolver eventResolver;

    private int nodeCounter = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        eventResolver = FindFirstObjectByType<EventResolver>();
        EnterNode(startingNode);
    }

    // =================== ENTRAR A UN NODO ===================

    public void EnterNode(ProgressionNode node)
    {
        currentNode = node;

        Debug.Log("=================================");
        Debug.Log($"[PROGRESSION] Estás en nodo: {node.nodeId}");
        Debug.Log("=================================");

        // Si hay narrativa fija al entrar
        if (node.entryNarrative != null)
        {
            GameFlowManager.Instance.EnterNarrative(node.entryNarrative);
        }
        else
        {
            ShowOptions();
        }
    }

    // =================== MOSTRAR OPCIONES ===================

    public void ShowOptions()
    {
        bool canGoBack = currentNode.previousNode != null;
        ProgressionUIController.Instance.Show(canGoBack);
    }

    // =================== ELEGIR OPCIÓN ===================

    public void ChooseOption(int option)
    {
        switch (option)
        {
            case 0:
                Advance();
                break;

            case 1:
                GoBack();
                break;

            default:
                Debug.LogWarning("Opción inválida");
                break;
        }
    }

    // =================== AVANZAR ===================

    private void Advance()
    {
        Debug.Log("[PROGRESSION] Avanzando…");

        // Crear nuevo nodo dinámico
        ProgressionNode newNode = ScriptableObject.CreateInstance<ProgressionNode>();
        nodeCounter++;

        newNode.nodeId = "Nodo_" + nodeCounter;
        newNode.previousNode = currentNode;

        // Copiar probabilidades
        newNode.combatChance = currentNode.combatChance;
        newNode.lootChance = currentNode.lootChance;
        newNode.narrativeChance = currentNode.narrativeChance;

        currentNode = newNode;

        ResolveAdvanceEvent(newNode);
    }

    // =================== RETROCEDER (COMBATE FORZADO) ===================

    private void GoBack()
    {
        if (currentNode.previousNode == null)
        {
            Debug.Log("No puedes retroceder más.");
            return;
        }

        Debug.Log("[PROGRESSION] Retrocediendo… COMBATE FORZADO");

        currentNode = currentNode.previousNode;

        CombatEvent combat = GameFlowManager.Instance.GetRandomCombatEvent();
        eventResolver.ResolveEvent(combat);
    }

    // =================== RESOLVER EVENTO AL AVANZAR ===================

    private void ResolveAdvanceEvent(ProgressionNode node)
    {
        float roll = Random.value;

        Debug.Log($"[PROGRESSION] Tirada de evento: {roll}");

        if (roll <= node.combatChance)
        {
            Debug.Log("[PROGRESSION] Encuentro de COMBATE");

            CombatEvent combat = GameFlowManager.Instance.GetRandomCombatEvent();
            eventResolver.ResolveEvent(combat);
        }
        else if (roll <= node.combatChance + node.lootChance)
        {
            Debug.Log("[PROGRESSION] Encuentro de LOOT");

            GameEvent loot = GameFlowManager.Instance.GetRandomLootEvent();
            eventResolver.ResolveEvent(loot);
        }
        else if (roll <= node.combatChance + node.lootChance + node.narrativeChance)
        {
            Debug.Log("[PROGRESSION] Encuentro NARRATIVO");

            NarrativeNode narrative = GameFlowManager.Instance.GetRandomNarrativeNode();
            GameFlowManager.Instance.EnterNarrative(narrative);
        }
        else
        {
            Debug.Log("[PROGRESSION] Nodo vacío");

            ShowOptions();
        }
    }

    // =================== CALLBACK DESDE EVENTOS ===================

    // Se llama al terminar combate, loot o narrativa
    public void ContinueAfterEvent()
    {
        Debug.Log("[PROGRESSION] Continuando después del evento");
        ShowOptions();
    }

    // =================== CALLBACK DESDE NARRATIVA ===================

    public void ResolveNarrativeChoice(NarrativeChoiceData choice)
    {
        // Evento asociado a la elección
        if (choice.choiceEvent != null)
        {
            eventResolver.ResolveEvent(choice.choiceEvent);
            return;
        }

        // Forzar combate
        if (choice.forceCombat)
        {
            Debug.Log("[PROGRESSION] Elección fuerza combate");

            CombatEvent combat = GameFlowManager.Instance.GetRandomCombatEvent();
            eventResolver.ResolveEvent(combat);
            return;
        }

        // Retroceder
        if (choice.goBack)
        {
            GoBack();
            return;
        }

        // Avanzar a nodo narrativo específico
        if (choice.nextNode != null)
        {
            // Si el nodo narrativo debe ser mostrado, usa EnterNarrative
            GameFlowManager.Instance.EnterNarrative(choice.nextNode);
            return;
        }

        // Si no hace nada especial, continuar loop
        ContinueAfterEvent();
    }
}
