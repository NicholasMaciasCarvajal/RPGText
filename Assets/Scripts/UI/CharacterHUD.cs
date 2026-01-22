using UnityEngine;
using UnityEngine.UI;

public class CharacterHUD : MonoBehaviour
{
    public Text infoText;

    private CharacterBase character;

    public void Bind(CharacterBase target)
    {
        character = target;
        Refresh();
    }

    private void Update()
    {
        if (character != null)
            Refresh();
    }

    private void Refresh()
    {
        if (!character.isAlive)
        {
            infoText.text = $"{character.name} [MUERTO]";
            return;
        }

        infoText.text =
            $"{character.name}\n" +
            $"HP: {character.currentHealth} / {character.maxHealth}\n" +
            $"EN: {character.currentEnergy} / {character.maxEnergy}";
    }
}
