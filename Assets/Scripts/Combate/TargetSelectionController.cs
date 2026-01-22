using UnityEngine;

public class TargetSelectionController : MonoBehaviour
{
    public static TargetSelectionController Instance;

    private PlayerTurnController localTurnController;
    private int pendingAbilityIndex = -1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // llamado cuando el jugador elige habilidad
    public void BeginTargetSelection(PlayerTurnController controller, int abilityIndex)
    {
        localTurnController = controller;
        pendingAbilityIndex = abilityIndex;

        Debug.Log("[CLIENT] Selecciona un objetivo…");

        // aquí luego:
        // - activar highlights en enemigos
        // - cambiar cursor
    }

    // llamado cuando haces click en un enemigo
    public void SelectTarget(EnemyCharacter enemy)
    {
        if (localTurnController == null) return;
        if (enemy == null || !enemy.isAlive) return;

        localTurnController
            .SubmitAbilityWithTarget(pendingAbilityIndex, enemy);

        // limpiar estado
        localTurnController = null;
        pendingAbilityIndex = -1;

        // aquí luego:
        // - quitar highlights
    }
}
