using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyClickable : MonoBehaviour, IPointerClickHandler
{
    private EnemyCharacter enemy;

    private void Awake()
    {
        enemy = GetComponent<EnemyCharacter>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[ENEMY] Click detectado en enemigo");

        if (enemy == null)
        {
            Debug.LogError("[ENEMY] EnemyCharacter es null");
            return;
        }

        Debug.Log("[ENEMY] Enviando enemigo a TargetSelectionController");

        TargetSelectionController.Instance.SelectTarget(enemy);
    }
}
