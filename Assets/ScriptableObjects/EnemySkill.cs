using UnityEngine;

[CreateAssetMenu(fileName = "EnemySkill", menuName = "Scriptable Objects/EnemySkill")]
public class EnemySkill : ScriptableObject
{
    public string skillName;
    public int minDamage;
    public int maxDamage;
    public float failChance = 0.1f; // 10% por defecto

    public int RollDamage()
    {
        return Random.Range(minDamage, maxDamage + 1);
    }
}
