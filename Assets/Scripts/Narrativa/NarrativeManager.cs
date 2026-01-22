using UnityEngine;
using System.Collections.Generic;

public class NarrativeManager : MonoBehaviour
{
    public static NarrativeManager Instance;

    [Header("Nodo inicial")]
    public NarrativeNode startNode;

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

    public void StartNarrative()
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

        // Si el nodo tiene evento al entrar
        if (node.onEnterEvent != null)
        {
            EventResolver resolver = FindObjectOfType<EventResolver>();
            resolver.ResolveEvent(node.onEnterEvent);
        }
    }

    // =================== ELECCIÓN ===================

    public void SelectChoice(NarrativeChoiceData choice)
    {
        // Evento al elegir
        if (choice.choiceEvent != null)
        {
            EventResolver resolver = FindObjectOfType<EventResolver>();
            resolver.ResolveEvent(choice.choiceEvent);
            return;
        }

        // Forzar combate
        if (choice.forceCombat)
        {
            Debug.Log("[NARRATIVE] Opción fuerza combate");
            GameFlowManager.Instance.EnterRandomCombat((CombatEvent)null);
            return;
        }

        // Retroceder
        if (choice.goBack)
        {
            if (history.Count > 0)
            {
                var previous = history.Pop();
                EnterNode(previous);
            }
            return;
        }

        // Avanzar normal
        if (choice.nextNode != null)
        {
            history.Push(currentNode);
            EnterNode(choice.nextNode);
        }
        else
        {
            Debug.LogWarning("Choice sin nextNode definido");
        }
    }
}
