using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyUnit : CharacterBase
{
    [Header("Pool de habilidades disponibles")]
    public List<Ability> skillPool;

    [Header("Habilidades activas")]
    public List<Ability> abilities = new List<Ability>();

    public int minSkills = 1;
    public int maxSkills = 3;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (IsServer)
            RandomizeSkills();
    }

    private void RandomizeSkills()
    {
        abilities.Clear();

        int skillsToAssign = Random.Range(minSkills, maxSkills + 1);

        var poolCopy = new List<Ability>(skillPool);

        for (int i = 0; i < skillsToAssign && poolCopy.Count > 0; i++)
        {
            int index = Random.Range(0, poolCopy.Count);
            abilities.Add(poolCopy[index]);
            poolCopy.RemoveAt(index);
        }
    }
}
