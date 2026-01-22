using UnityEngine;

public class EventResolver : MonoBehaviour
{
    public void ResolveEvent(GameEvent gameEvent)
    {
        Debug.Log($"[EVENT] Resolviendo evento: {gameEvent.eventName}");
        ProgressionUIController.Instance.Hide();

        switch (gameEvent.eventType)
        {
            case GameEvent.EventType.Combat:
                ResolveCombat(gameEvent as CombatEvent);
                break;

            case GameEvent.EventType.Loot:
                ResolveLoot(gameEvent);
                break;

            case GameEvent.EventType.Empty:
                ResolveEmpty();
                break;

            case GameEvent.EventType.Narrative:
                GameFlowManager.Instance.EnterNarrative();
                break;

            default:
                Debug.LogWarning("Tipo de evento desconocido");
                break;
        }
    }
    private void ResolveCombat(CombatEvent combatEvent)
    {
        GameFlowManager.Instance.EnterCombat(combatEvent);
    }


    private void ResolveLoot(GameEvent lootEvent)
    {
        Debug.Log("[EVENT] Loot (pendiente implementar)");
        GameFlowManager.Instance.EnterNarrative();
    }

    private void ResolveEmpty()
    {
        Debug.Log("[EVENT] Evento vacío");
        GameFlowManager.Instance.EnterNarrative();
    }
}
