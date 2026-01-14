using UnityEngine;

public class EnemyClickable : MonoBehaviour
{
    private EnemyCharacter enemy;

    private void Awake()
    {
        enemy = GetComponent<EnemyCharacter>();
    }

    private void OnMouseDown()
    {
        if (TargetSelectionController.Instance != null)
            TargetSelectionController.Instance.SelectTarget(enemy);
    }
}
