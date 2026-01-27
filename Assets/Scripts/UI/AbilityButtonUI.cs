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
        Debug.Log("[UI] Click en botón de habilidad");

        if (controller == null)
        {
            Debug.LogError("[UI] controller es NULL en AbilityButtonUI");
            return;
        }

        Debug.Log($"[UI] Enviando habilidad índice {abilityIndex} al PlayerTurnController");

        controller.SendAbilityChoice(abilityIndex);
    }

}
