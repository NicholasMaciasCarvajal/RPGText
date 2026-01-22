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
        if (enemy == null || !enemy.isAlive)
            return;

        TargetSelectionController.Instance.SelectTarget(enemy);
    }
}
