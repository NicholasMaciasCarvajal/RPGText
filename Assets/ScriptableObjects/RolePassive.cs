using UnityEngine;

[CreateAssetMenu(fileName = "RolePassive", menuName = "Scriptable Objects/RolePassive")]
public abstract class RolePassive : ScriptableObject
{
    public abstract void Apply(PlayerCharacter player);
}
