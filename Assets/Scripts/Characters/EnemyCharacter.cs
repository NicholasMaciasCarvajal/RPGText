using UnityEngine;

public class EnemyCharacter : CharacterBase
{
    public void TakeTurn()
    {
        if (!isAlive) return;

        Debug.Log($"{name} está decidiendo su acción...");

        // IA SIMPLE (placeholder)
        AttackPlayer();
    }

    private void AttackPlayer()
    {
        PlayerCharacter target = ChooseTarget();

        if (target == null) return;

        target.TakeDamage(attack);

        GameManager.Instance.turnManager.EndTurn();
    }

    private PlayerCharacter ChooseTarget()
    {
        if (GameManager.Instance.player1.isAlive)
            return GameManager.Instance.player1;

        if (GameManager.Instance.player2.isAlive)
            return GameManager.Instance.player2;

        return null;
    }
}
