public enum ActionType
{
    Ability,
    Item
}

[System.Serializable]
public class ActionRequest
{
    public ulong playerNetworkId;
    public ActionType actionType;

    public int abilityIndex;
    public int itemIndex;

    public ulong targetNetworkId;
}
