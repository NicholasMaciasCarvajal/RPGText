using UnityEngine;

public class NarrativeManager : MonoBehaviour
{
    public static NarrativeManager Instance;

    [Header("Nodo inicial")]
    public NarrativeNode startingNode;

    private NarrativeNode currentNode;

    private EventResolver eventResolver;

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
    }

    // Llamado por GameFlowManager al entrar en narrativa
    public void StartNarrative(NarrativeNode node = null)
    {
        if (node == null)
            currentNode = startingNode;
        else
            currentNode = node;

        ShowCurrentNode();
    }

    private void ShowCurrentNode()
    {
        Debug.Log("=================================");
        Debug.Log("[NARRATIVE] " + currentNode.narrativeText);

        for (int i = 0; i < currentNode.choices.Count; i++)
        {
            Debug.Log($"[{i}] {currentNode.choices[i].choiceText}");
        }

        ProgressionManager.Instance.ContinueAfterEvent();


        Debug.Log("=================================");
    }

    // Esto luego se conectará a botones UI
    public void ChooseOption(int index)
    {
        if (index < 0 || index >= currentNode.choices.Count)
            return;

        NarrativeChoice choice = currentNode.choices[index];

        Debug.Log($"[NARRATIVE] Elegiste: {choice.choiceText}");

        // Si hay evento, resolver evento primero
        if (choice.linkedEvent != null)
        {
            eventResolver.ResolveEvent(choice.linkedEvent);
        }
        else if (choice.nextNode != null)
        {
            // continuar narrativa directamente
            StartNarrative(choice.nextNode);
        }
        else
        {
            Debug.LogWarning("Opción sin evento ni siguiente nodo.");
        }
    }

    // Llamado cuando termina un combate / loot / evento
    public void ContinueAfterEvent(NarrativeNode nextNode)
    {
        if (nextNode != null)
            StartNarrative(nextNode);
        else
            Debug.Log("[NARRATIVE] Fin de narrativa (no hay siguiente nodo)");
    }
}
