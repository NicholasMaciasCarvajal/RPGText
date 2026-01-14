using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkBattleManager : NetworkBehaviour
{
    public EnemyUnit[] enemies;

    [Serializable]
    public struct QueuedAction : INetworkSerializable, IEquatable<QueuedAction>
    {
        public ulong playerClientId;
        public int abilityId;
        public ulong targetNetworkId;
        public double timeSubmitted;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref playerClientId);
            serializer.SerializeValue(ref abilityId);
            serializer.SerializeValue(ref targetNetworkId);
            serializer.SerializeValue(ref timeSubmitted);
        }

        public bool Equals(QueuedAction other)
        {
            return playerClientId == other.playerClientId &&
                   abilityId == other.abilityId &&
                   targetNetworkId == other.targetNetworkId &&
                   timeSubmitted.Equals(other.timeSubmitted);
        }

        public override bool Equals(object obj)
        {
            return obj is QueuedAction other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(playerClientId, abilityId, targetNetworkId, timeSubmitted);
        }
    }

    private NetworkList<QueuedAction> queuedActions;

    private TurnManager turnManager;

    private void Awake()
    {
        queuedActions = new NetworkList<QueuedAction>();
    }

    private void Start()
    {
        // turnManager = FindObjectOfType<TurnManager>();
        turnManager = FindFirstObjectByType<TurnManager>();
    }

    // =================== CLIENT - SERVER ===================

    // client envía su decisión
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    // [ServerRpc(RequireOwnership = false)]
    public void SubmitPlayerActionServerRpc(int abilityId, ulong targetNetworkId, ServerRpcParams rpcParams = default)
    {
        if (!IsServer) return;

        var clientId = rpcParams.Receive.SenderClientId;

        // evitar más de 1 acción por jugador
        foreach (var qa in queuedActions)
        {
            if (qa.playerClientId == clientId)
                return;
        }

        var action = new QueuedAction
        {
            playerClientId = clientId,
            abilityId = abilityId,
            targetNetworkId = targetNetworkId,
            timeSubmitted = NetworkManager.ServerTime.Time
        };

        queuedActions.Add(action);

        // cuando ya tenemos 2, resolvemos
        if (queuedActions.Count >= 2)
        {
            ResolvePlayersPhaseServer();
        }
    }

    // =================== RESOLVER JUGADORES ===================

    private void ResolvePlayersPhaseServer()
    {
        if (!IsServer) return;

        Debug.Log("[SERVER] Resolviendo fase de jugadores…");

        // copiar manualmente a lista normal
        List<QueuedAction> ordered = new List<QueuedAction>();

        foreach (var qa in queuedActions)
        {
            ordered.Add(qa);
        }

        // ordenar por timestamp
        ordered.Sort((a, b) => a.timeSubmitted.CompareTo(b.timeSubmitted));

        foreach (var action in ordered)
        {
            ExecuteQueuedActionServer(action);
        }

        queuedActions.Clear();

        // avanzar turno
        turnManager.EndTurnServer();
    }


    private void ExecuteQueuedActionServer(QueuedAction action)
    {
        // get caster player object via clientId
        var casterObj = NetworkManager.Singleton.ConnectedClients[action.playerClientId].PlayerObject;
        var caster = casterObj.GetComponent<PlayerCharacter>();

        // get target via network object id
        var targetObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[action.targetNetworkId];
        var target = targetObj.GetComponent<CharacterBase>();

        // get ability
        var ability = caster.abilities[action.abilityId];

        // execute ability
        AbilityExecutor.ExecuteAbility(caster, target, ability);

        // notify clients
        BroadcastActionResolutionClientRpc(
            casterObj.NetworkObjectId,
            targetObj.NetworkObjectId,
            action.abilityId
        );
    }


    // =================== FEEDBACK A CLIENTES ===================

    [ClientRpc]
    private void BroadcastActionResolutionClientRpc(
        ulong casterId,
        ulong targetId,
        int abilityIndex)
    {
        var caster = NetworkManager.Singleton.SpawnManager.SpawnedObjects[casterId]
            .GetComponent<CharacterBase>();

        var target = NetworkManager.Singleton.SpawnManager.SpawnedObjects[targetId]
            .GetComponent<CharacterBase>();

        Debug.Log($"{caster.name} ejecutó habilidad {abilityIndex} sobre {target.name}");

        // aquí puedes:
        // reproducir animaciones
        // actualizar UI
        // mostrar texto flotante
    }


    // =================== ENEMIGOS ===================

    public void ResolveEnemiesPhaseServer()
    {
        if (!IsServer) return;

        Debug.Log("[SERVER] Resolviendo turno de enemigos…");

        StartCoroutine(EnemiesTurnRoutine());
    }

    private IEnumerator EnemiesTurnRoutine()
    {
        foreach (var enemy in enemies.Where(e => e != null))
        {
            yield return new WaitForSeconds(0.5f);

            ExecuteEnemyAction(enemy);
        }

        GameManager.Instance.turnManager.EndTurnServer();
    }

    private void ExecuteEnemyAction(EnemyUnit enemy)
    {
        if (enemy.activeSkills.Count == 0)
        {
            Debug.Log("Enemy has no skills");
            return;
        }

        // Seleccionar skill
        var skill = enemy.activeSkills[Random.Range(0, enemy.activeSkills.Count)];

        // Seleccionar objetivo (Player 1 o 2)
        var target = Random.value < 0.5f
            ? GameManager.Instance.player1
            : GameManager.Instance.player2;

        // Checar fallo
        if (Random.value < skill.failChance)
        {
            Debug.Log($"Enemy failed using {skill.skillName}");
            return;
        }

        int dmg = skill.RollDamage();

        target.TakeDamage(dmg);

        Debug.Log($"Enemy used {skill.skillName} on {target.name} for {dmg} damage");
    }

    private PlayerCharacter GetRandomAlivePlayer()
    {
        List<PlayerCharacter> alivePlayers = new List<PlayerCharacter>();

        if (GameManager.Instance.player1 != null && GameManager.Instance.player1.isAlive)
            alivePlayers.Add(GameManager.Instance.player1);

        if (GameManager.Instance.player2 != null && GameManager.Instance.player2.isAlive)
            alivePlayers.Add(GameManager.Instance.player2);

        if (alivePlayers.Count == 0)
            return null;

        int index = UnityEngine.Random.Range(0, alivePlayers.Count);
        return alivePlayers[index];
    }

}
