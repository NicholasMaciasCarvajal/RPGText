using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NarrativeNode", menuName = "Scriptable Objects/NarrativeNode")]
public class NarrativeNode : ScriptableObject
{
    [Header("Texto del nodo")]
    [TextArea(4, 8)]
    public string narrativeText;

    [Header("Opciones disponibles")]
    public NarrativeChoiceData[] choices;

    [Header("Fondo")]
    public Sprite backgroundImage;

    [Header("Evento al entrar (opcional)")]
    public GameEvent onEnterEvent;
}
