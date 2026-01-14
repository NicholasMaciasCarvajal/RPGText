using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyCharacter : CharacterBase
{
    [Header("Enemy AI")]
    public List<Ability> abilities = new List<Ability>();

    // probabilidad base si habilidad no define la suya
    [Range(0f, 1f)]
    public float defaultHitChance = 0.85f;

    public override void TakeTurn()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

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
            Debug.Log("No tengo habilidades. Ataque básico.");
            BasicAttack(target);
            return;
        }

        UseRandomAbility(target);
    }

    private void BasicAttack(PlayerCharacter target)
    {
        float roll = Random.value;

        if (roll <= defaultHitChance)
        {
            target.TakeDamageServerRpc(attack);
            Debug.Log($"{name} acertó ataque básico a {target.name}");
        }
        else
        {
            Debug.Log($"{name} falló ataque básico a {target.name}");
        }
    }

    public void UseRandomAbility(PlayerCharacter target)
    {
        Ability ability = abilities[Random.Range(0, abilities.Count)];

        // Probabilidad (si la habilidad tiene la suya, úsala)
        float hitChance = ability.hitChance > 0
            ? ability.hitChance
            : defaultHitChance;

        float roll = Random.value;

        if (roll > hitChance)
        {
            Debug.Log($"{name} falló {ability.abilityName} contra {target.name}");
            return;
        }

        int damage = ability.RollDamage();

        target.TakeDamageServerRpc(damage);

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
}
