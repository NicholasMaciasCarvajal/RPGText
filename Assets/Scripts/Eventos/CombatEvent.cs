using UnityEngine;

[CreateAssetMenu(fileName = "CombatEvent", menuName = "Scriptable Objects/CombatEvent")]
public class CombatEvent : GameEvent
{
    [Header("Enemigos del combate")]
    public CharacterBase[] enemies;

    private void OnEnable()
    {
        eventType = EventType.Combat;
    }
}
