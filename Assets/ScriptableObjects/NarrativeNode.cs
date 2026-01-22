using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NarrativeNode", menuName = "Scriptable Objects/NarrativeNode")]
public class NarrativeNode : ScriptableObject
{
    [Header("Texto narrativo")]
    [TextArea(5, 10)]
    public string narrativeText;

    [Header("Opciones disponibles")]
    public List<NarrativeChoice> choices = new List<NarrativeChoice>();
}
