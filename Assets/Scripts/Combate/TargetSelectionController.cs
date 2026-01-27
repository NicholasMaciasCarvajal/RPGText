using UnityEngine;

public class TargetSelectionController : MonoBehaviour
{
    public static TargetSelectionController Instance;

    private PlayerTurnController currentController;
    private int currentAbilityIndex;
    private bool selecting = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // llamado desde PlayerTurnController cuando eliges habilidad
    public void BeginTargetSelection(PlayerTurnController controller, int abilityIndex)
    {
        currentController = controller;
        currentAbilityIndex = abilityIndex;
        selecting = true;

        Debug.Log("[TARGET] Selecciona un enemigo haciendo click");
    }

    // llamado desde EnemyClickable
    public void SelectTarget(EnemyCharacter enemy)
    {
        if (!selecting)
        {
            Debug.LogWarning("[TARGET] No estaba en modo selección");
            return;
        }

        if (currentController == null)
        {
            Debug.LogError("[TARGET] currentController es NULL — no hay jugador activo");
            return;
        }

        Debug.Log($"[TARGET] Objetivo seleccionado: {enemy.name}");

        selecting = false;

        // AQUÍ ESTABA TU BUG
        currentController.SubmitAbilityWithTarget(currentAbilityIndex, enemy);

        // limpiar
        currentController = null;
    }
}
