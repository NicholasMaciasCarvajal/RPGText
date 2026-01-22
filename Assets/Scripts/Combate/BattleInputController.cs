using UnityEngine;

public class BattleInputController : MonoBehaviour
{
    public PlayerTurnController localTurnController;

    public void OnClickAbility(int index)
    {
        if (localTurnController == null) return;

        localTurnController.SendAbilityChoice(index);
    }

    public void OnClickItem(int index)
    {
        if (localTurnController == null) return;

        localTurnController.SendItemChoice(index);
    }
}
