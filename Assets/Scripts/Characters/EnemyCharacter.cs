using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyCharacter : CharacterBase, IPointerClickHandler
{
    [Header("Enemy AI")]
    public List<Ability> abilities = new List<Ability>();

    [Range(0f, 1f)]
    public float defaultHitChance = 0.85f;

    public override void TakeTurn()
    {
        if (!IsServer) return;
        if (!isAlive) return;

        Debug.Log($"{name} está decidiendo su acción…");

        ExecuteRandomAction();
    }

    private void ExecuteRandomAction()
    {
        PlayerCharacter target = ChooseTarget();
        if (target == null)
        {
            Debug.Log("No hay objetivos vivos.");
            return;
        }

        if (abilities.Count == 0)
        {
            BasicAttack(target);
            return;
        }

        UseRandomAbility(target);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (TargetSelectionController.Instance != null)
        {
            TargetSelectionController.Instance.SelectTarget(this);
        }
    }

    private void BasicAttack(PlayerCharacter target)
    {
        float roll = Random.value;

        if (roll <= defaultHitChance)
        {
            int damage = attack;

            // daño en servidor
            target.TakeDamage(damage);

            // notificar a clientes
            NotifyEnemyAttackClientRpc(
                NetworkObjectId,
                target.GetComponent<NetworkObject>().NetworkObjectId,
                damage,
                true
            );

            Debug.Log($"{name} acertó ataque básico a {target.name}");
        }
        else
        {
            NotifyEnemyAttackClientRpc(
                NetworkObjectId,
                target.GetComponent<NetworkObject>().NetworkObjectId,
                0,
                false
            );

            Debug.Log($"{name} falló ataque básico a {target.name}");
        }
    }

    private void UseRandomAbility(PlayerCharacter target)
    {
        Ability ability = abilities[Random.Range(0, abilities.Count)];

        float hitChance = ability.hitChance > 0
            ? ability.hitChance
            : defaultHitChance;

        float roll = Random.value;

        if (roll > hitChance)
        {
            NotifyEnemyAttackClientRpc(
                NetworkObjectId,
                target.GetComponent<NetworkObject>().NetworkObjectId,
                0,
                false
            );

            Debug.Log($"{name} falló {ability.abilityName} contra {target.name}");
            return;
        }

        // ejecutar habilidad real y obtener daño real
        int damage = AbilityExecutor.ExecuteAbility(this, target, ability);

        // notificar clientes con el daño real aplicado
        NotifyEnemyAttackClientRpc(
            NetworkObjectId,
            target.GetComponent<NetworkObject>().NetworkObjectId,
            damage,
            true
        );

        Debug.Log($"{name} usó {ability.abilityName} contra {target.name} e hizo {damage} daño");
    }


    private PlayerCharacter ChooseTarget()
    {
        var gm = GameManager.Instance;

        List<PlayerCharacter> alive = new List<PlayerCharacter>();

        if (gm.player1 != null && gm.player1.isAlive) alive.Add(gm.player1);
        if (gm.player2 != null && gm.player2.isAlive) alive.Add(gm.player2);

        if (alive.Count == 0)
            return null;

        int index = Random.Range(0, alive.Count);
        return alive[index];
    }

    // =================== SYNC A CLIENTES ===================

    [ClientRpc]
    private void NotifyEnemyAttackClientRpc(
        ulong enemyId,
        ulong targetId,
        int damage,
        bool hit)
    {
        var enemy = NetworkManager.Singleton.SpawnManager
            .SpawnedObjects[enemyId].GetComponent<CharacterBase>();

        var target = NetworkManager.Singleton.SpawnManager
            .SpawnedObjects[targetId].GetComponent<CharacterBase>();

        if (hit)
        {
            Debug.Log($"[CLIENT] {enemy.name} atacó a {target.name} por {damage}");
        }
        else
        {
            Debug.Log($"[CLIENT] {enemy.name} falló ataque contra {target.name}");
        }

        // aquí luego:
        // animaciones
        // texto flotante
        // sonido
    }
}
