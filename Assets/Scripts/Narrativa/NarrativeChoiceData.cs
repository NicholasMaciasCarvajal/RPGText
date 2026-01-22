using UnityEngine;

[System.Serializable]
public class NarrativeChoiceData
{
    public string choiceText;

    [Header("Nodo destino")]
    public NarrativeNode nextNode;

    [Header("Evento al elegir (opcional)")]
    public GameEvent choiceEvent;

    [Header("Tipo especial")]
    public bool forceCombat; 
    public bool goBack;         
}
