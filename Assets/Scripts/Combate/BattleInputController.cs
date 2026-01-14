using UnityEngine;

public class BattleInputController : MonoBehaviour
{
    public PlayerTurnController localTurnController;

    public void OnClickAbility(int index)
    {
        localTurnController.SendAbilityChoice(index);
    }

    public void OnClickItem(int index)
    {
        localTurnController.SendItemChoice(index);
    }
}
