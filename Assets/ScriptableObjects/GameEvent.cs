using UnityEngine;

public abstract class GameEvent : ScriptableObject
{
    public enum EventType
    {
        Combat,
        Loot,
        Empty,
        Narrative
    }

    public string eventName;
    public EventType eventType;
}
