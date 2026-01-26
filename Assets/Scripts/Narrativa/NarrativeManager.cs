using UnityEngine;
using System.Collections.Generic;

public class NarrativeManager : MonoBehaviour
{
    public static NarrativeManager Instance;

    private NarrativeNode currentNode;
    private Stack<NarrativeNode> history = new Stack<NarrativeNode>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // =================== ENTRAR EN NARRATIVA ===================

    public void StartNarrative(NarrativeNode startNode)
    {
        currentNode = startNode;
        history.Clear();

        EnterNode(currentNode);
    }

    private void EnterNode(NarrativeNode node)
    {
        currentNode = node;

        NarrativeHUDController.Instance.Show();
        NarrativeHUDController.Instance.ShowNode(node, this);
    }

    // =================== ELECCIÓN DEL JUGADOR ===================

    public void SelectChoice(NarrativeChoiceData choice)
    {
        // Guardar historial si avanza a otro nodo
        if (choice.nextNode != null)
        {
            history.Push(currentNode);
        }

        // Delegar toda la lógica al ProgressionManager
        NarrativeHUDController.Instance.Hide();
        ProgressionManager.Instance.ResolveNarrativeChoice(choice);
    }

    // =================== RETROCEDER DENTRO DE NARRATIVA (OPCIONAL) ===================

    public void GoBackNarrative()
    {
        if (history.Count > 0)
        {
            var previous = history.Pop();
            EnterNode(previous);
        }
    }
}
