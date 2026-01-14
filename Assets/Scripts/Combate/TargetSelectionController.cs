using UnityEngine;

public class TargetSelectionController : MonoBehaviour
{
    public static TargetSelectionController Instance;

    private PlayerTurnController localTurnController;
    private int pendingAbilityIndex = -1;

    private void Awake()
    {
        Instance = this;
    }

    public void BeginTargetSelection(PlayerTurnController controller, int abilityIndex)
    {
        localTurnController = controller;
        pendingAbilityIndex = abilityIndex;

        Debug.Log("Selecciona un objetivo…");
    }

    public void SelectTarget(EnemyCharacter enemy)
    {
        if (localTurnController == null) return;

        localTurnController.SubmitAbilityWithTarget(pendingAbilityIndex, enemy);

        localTurnController = null;
        pendingAbilityIndex = -1;
    }
}
