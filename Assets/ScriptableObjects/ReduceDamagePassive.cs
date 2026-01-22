using UnityEngine;

[CreateAssetMenu(fileName = "ReduceDamagePassive", menuName = "Scriptable Objects/ReduceDamagePassive")]
public class ReduceDamagePassive : RolePassive
{
    public int percent;

    public override void Apply(PlayerCharacter player)
    {
        player.defense += percent;
    }
}
