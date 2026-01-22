using UnityEngine;

[System.Serializable]
public class NarrativeChoice
{
    [TextArea]
    public string choiceText;

    // Evento que se ejecuta al elegir esta opción
    public GameEvent linkedEvent;

    // Nodo al que se pasa después (si no hay evento o después del evento)
    public NarrativeNode nextNode;
}
