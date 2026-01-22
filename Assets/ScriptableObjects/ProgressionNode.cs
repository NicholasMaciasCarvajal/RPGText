using UnityEngine;

[CreateAssetMenu(fileName = "ProgressionNode", menuName = "Scriptable Objects/ProgressionNode")]
public class ProgressionNode : ScriptableObject
{
    [Header("Identificación")]
    public string nodeId;

    [Header("Nodo anterior")]
    public ProgressionNode previousNode;

    [Header("Probabilidades al AVANZAR (0–1)")]

    [Range(0f, 1f)] public float combatChance = 0.5f;
    [Range(0f, 1f)] public float lootChance = 0.3f;
    [Range(0f, 1f)] public float narrativeChance = 0.2f;

    // Opcional: narrativa fija al entrar
    public NarrativeNode entryNarrative;
}
