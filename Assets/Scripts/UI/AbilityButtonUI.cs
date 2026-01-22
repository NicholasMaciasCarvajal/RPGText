using UnityEngine;
using UnityEngine.UI;

public class AbilityButtonUI : MonoBehaviour
{
    public Text label;
    private int abilityIndex;
    private PlayerTurnController controller;

    public void Setup(PlayerTurnController ctrl, int index, Ability ability)
    {
        controller = ctrl;
        abilityIndex = index;

        label.text = ability.abilityName + $" (EN {ability.energyCost})";
    }

    public void OnClick()
    {
        controller.SendAbilityChoice(abilityIndex);
    }
}
