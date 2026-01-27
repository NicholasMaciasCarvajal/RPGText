using UnityEngine;

public class EventResolver : MonoBehaviour
{
    public void ResolveEvent(GameEvent gameEvent)
    {
        if (!Unity.Netcode.NetworkManager.Singleton.IsServer)
            return;

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
                GameFlowManager.Instance.EnterNarrative(
                    GameFlowManager.Instance.GetRandomNarrativeNode());
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
        // Solución: obtener un nodo narrativo aleatorio y pasarlo como argumento
        GameFlowManager.Instance.EnterNarrative(GameFlowManager.Instance.GetRandomNarrativeNode());
    }

    private void ResolveEmpty()
    {
        Debug.Log("[EVENT] Evento vacío");
        // Solución: obtener un nodo narrativo aleatorio y pasarlo como argumento
        GameFlowManager.Instance.EnterNarrative(GameFlowManager.Instance.GetRandomNarrativeNode());
    }
}
