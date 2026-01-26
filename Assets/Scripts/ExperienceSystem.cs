using Unity.Netcode;
using UnityEngine;

public class ExperienceSystem : NetworkBehaviour
{
    [Header("Level")]
    public int level = 1;

    [Header("Experience")]
    public int currentXP = 0;
    public int xpToNextLevel = 100;

    private PlayerCharacter player;

    private void Awake()
    {
        player = GetComponent<PlayerCharacter>();
    }

    // ================= XP =================

    public void AddExperience(int amount)
    {
        if (!IsServer) return;

        currentXP += amount;
        Debug.Log($"{player.name} gana {amount} XP");

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        xpToNextLevel = CalculateNextXP(level);

        Debug.Log($"{player.name} sube a nivel {level}");

        ApplyLevelStats();
    }

    private int CalculateNextXP(int lvl)
    {
        // curva simple tipo RPG casual
        return 100 + (lvl - 1) * 50;
    }

    // ================= STATS =================

    private void ApplyLevelStats()
    {
        if (player.roleData == null)
        {
            Debug.LogWarning("Jugador sin rol asignado, usando escalado base");
            ApplyBaseGrowth();
            return;
        }

        ApplyRoleGrowth(player.roleData);

        // restaurar vida y energía al subir nivel (opcional, pero recomendado)
        player.currentHealth = player.maxHealth;
        player.currentEnergy = player.maxEnergy;
    }

    private void ApplyBaseGrowth()
    {
        player.maxHealth += 10;
        player.maxEnergy += 5;
        player.attack += 2;
        player.defense += 1;
    }

    private void ApplyRoleGrowth(RoleData role)
    {
        // aquí puedes balancear fácilmente cada rol
        player.maxHealth += 10 + role.bonusHealth / 5;
        player.maxEnergy += 5 + role.bonusEnergy / 5;
        player.attack += 2 + role.bonusAttack / 4;
        player.defense += 1 + role.bonusDefense / 4;
    }
}
