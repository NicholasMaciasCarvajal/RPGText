using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class NetworkBattleManager : NetworkBehaviour
{
    public EnemyCharacter[] enemies;

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

    public static NetworkBattleManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        queuedActions = new NetworkList<QueuedAction>();
    }

    private void Start()
    {
        // turnManager = FindObjectOfType<TurnManager>();
        turnManager = FindFirstObjectByType<TurnManager>();
    }

    // =================== CLIENT - SERVER ===================

    [ServerRpc(RequireOwnership = false)]
    public void SubmitPlayerActionServerRpc(
        int abilityId,
        ulong targetNetworkId,
        ServerRpcParams rpcParams = default)

    {
        if (!IsServer) return;

        var clientId = rpcParams.Receive.SenderClientId;

        // obtener jugador del cliente
        var casterObj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        var caster = casterObj.GetComponent<PlayerCharacter>();

        var targetObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[targetNetworkId];
        var target = targetObj.GetComponent<CharacterBase>();

        if (target == null || !target.isAlive)
            return;

        if (!caster.CanAct())
        {
            Debug.LogWarning("Jugador intentó actuar fuera de turno.");
            return;
        }

        // evitar atacar aliados por error
        if (target is PlayerCharacter && caster is PlayerCharacter)
        {
            Debug.LogWarning("Jugador intentó atacar aliado.");
            return;
        }

        if (abilityId < 0 || abilityId >= caster.abilities.Count)
            return;

        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(targetNetworkId))
            return;

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

        CombatHUDController.Instance.HideAbilities();

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

        CheckBattleEnd();

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
        
        CombatHUDController.Instance.SetTurnText("Turno de enemigos");

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

    private void ExecuteEnemyAction(EnemyCharacter enemy)
    {
        if (enemy == null || !enemy.isAlive)
            return;

        // elegir objetivo vivo
        PlayerCharacter target = GetRandomAlivePlayer();

        if (target == null)
        {
            Debug.Log("No hay jugadores vivos para atacar.");
            return;
        }

        // si no tiene habilidades, ataque básico
        if (enemy.abilities.Count == 0)
        {
            int damage = enemy.attack;
            target.TakeDamage(damage);

            CheckBattleEnd();

            Debug.Log($"Enemy {enemy.name} hizo ataque básico a {target.name} por {damage}");
            return;
        }

        // elegir habilidad aleatoria
        Ability ability = enemy.abilities[Random.Range(0, enemy.abilities.Count)];

        // chequeo de hit
        float hitChance = ability.hitChance > 0 ? ability.hitChance : enemy.defaultHitChance;

        if (Random.value > hitChance)
        {
            Debug.Log($"Enemy {enemy.name} falló {ability.abilityName} contra {target.name}");
            return;
        }

        // ejecutar habilidad real (con energía, efectos, estados, etc.)
        AbilityExecutor.ExecuteAbility(enemy, target, ability);

        CheckBattleEnd();

        Debug.Log($"Enemy {enemy.name} usó {ability.abilityName} contra {target.name}");
    }

    public void StartBattle(CombatEvent combatEvent)
    {
        if (!IsServer) return;

        Debug.Log("[BATTLE] Iniciando combate desde evento");

        // Conversión explícita de CharacterBase[] a EnemyCharacter[]
        enemies = combatEvent.enemies.Cast<EnemyCharacter>().ToArray();

        turnManager = FindFirstObjectByType<TurnManager>();
        turnManager.EndTurnServer(); // inicia turno
    }


    private void CheckBattleEnd()
    {
        bool enemiesAlive = enemies.Any(e => e != null && e.isAlive);
        bool playersAlive =
            GameManager.Instance.player1.isAlive ||
            GameManager.Instance.player2.isAlive;

        if (!enemiesAlive)
        {
            GiveExperienceToPlayers();
            GameFlowManager.Instance.EndCombatVictory();
        }
        else if (!playersAlive)
        {
            GameFlowManager.Instance.EndCombatDefeat();
        }
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

    private void GiveExperienceToPlayers()
    {
        int totalXP = 50 * enemies.Length;

        if (GameManager.Instance.player1 != null && GameManager.Instance.player1.isAlive)
        {
            GameManager.Instance.player1.experienceSystem.AddExperience(totalXP);
        }

        if (GameManager.Instance.player2 != null && GameManager.Instance.player2.isAlive)
        {
            GameManager.Instance.player2.experienceSystem.AddExperience(totalXP);
        }

        Debug.Log($"[XP] Cada jugador gana {totalXP} XP");
    }


}
