using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyUnit : NetworkBehaviour
{
    public int maxHp = 100;
    public int currentHp;

    [Header("Pool de habilidades disponibles")]
    public List<EnemySkill> skillPool;

    [Header("Habilidades activas")]
    public List<EnemySkill> activeSkills = new List<EnemySkill>();

    public int minSkills = 1;
    public int maxSkills = 3;

    private void Start()
    {
        currentHp = maxHp;

        if (IsServer)
            RandomizeSkills();
    }

    private void RandomizeSkills()
    {
        activeSkills.Clear();

        int skillsToAssign = Random.Range(minSkills, maxSkills + 1);

        var poolCopy = new List<EnemySkill>(skillPool);

        for (int i = 0; i < skillsToAssign && poolCopy.Count > 0; i++)
        {
            int index = Random.Range(0, poolCopy.Count);
            activeSkills.Add(poolCopy[index]);
            poolCopy.RemoveAt(index);
        }
    }
}
