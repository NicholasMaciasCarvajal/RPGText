using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CombatHUDController : MonoBehaviour
{
    public static CombatHUDController Instance;

    [Header("Turn")]
    public Text turnText;

    [Header("Players")]
    public Transform playersPanel;
    public GameObject characterHudPrefab;

    [Header("Enemies")]
    public Transform enemiesPanel;

    [Header("Abilities")]
    public Transform abilitiesPanel;
    public GameObject abilityButtonPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // =================== INICIALIZAR HUD ===================

    public void SetupBattleUI(
        PlayerCharacter p1,
        PlayerCharacter p2,
        EnemyCharacter[] enemies)
    {
        ClearPanels();

        CreateCharacterHUD(playersPanel, p1);
        CreateCharacterHUD(playersPanel, p2);

        foreach (var enemy in enemies)
        {
            if (enemy != null)
                CreateCharacterHUD(enemiesPanel, enemy);
        }
    }

    private void CreateCharacterHUD(Transform panel, CharacterBase character)
    {
        var obj = Instantiate(characterHudPrefab, panel);
        var hud = obj.GetComponent<CharacterHUD>();
        hud.Bind(character);
    }

    private void ClearPanels()
    {
        foreach (Transform t in playersPanel) Destroy(t.gameObject);
        foreach (Transform t in enemiesPanel) Destroy(t.gameObject);
        foreach (Transform t in abilitiesPanel) Destroy(t.gameObject);
    }

    // =================== TURNO ===================

    public void SetTurnText(string text)
    {
        turnText.text = text;
    }

    // =================== HABILIDADES ===================

    public void ShowAbilities(PlayerTurnController controller, List<Ability> abilities)
    {
        foreach (Transform t in abilitiesPanel)
            Destroy(t.gameObject);

        for (int i = 0; i < abilities.Count; i++)
        {
            var obj = Instantiate(abilityButtonPrefab, abilitiesPanel);
            var btn = obj.GetComponent<AbilityButtonUI>();
            btn.Setup(controller, i, abilities[i]);
        }
    }

    public void HideAbilities()
    {
        foreach (Transform t in abilitiesPanel)
            Destroy(t.gameObject);
    }
}
