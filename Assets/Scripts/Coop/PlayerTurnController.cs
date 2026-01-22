using Unity.Netcode;
using UnityEngine;

public class PlayerTurnController : NetworkBehaviour
{
    private PlayerCharacter player;

    private int selectedAbilityIndex = -1;
    private ulong selectedTargetId;

    private void Awake()
    {
        player = GetComponent<PlayerCharacter>();
    }

    // ================== CONTROL DE TURNO ==================

    public void ResetTurnInput()
    {
        if (!IsOwner) return;

        selectedAbilityIndex = -1;
        selectedTargetId = 0;

        player.EnableInput(true);

        Debug.Log("[CLIENT] Tu turno ha comenzado.");
    }

    // ================== DESDE UI ==================

    // llamado por BattleInputController
    public void SendAbilityChoice(int abilityIndex)
    {
        if (!IsOwner) return;
        if (!player.CanAct()) return;

        if (abilityIndex < 0 || abilityIndex >= player.abilities.Count)
            return;

        selectedAbilityIndex = abilityIndex;

        Debug.Log($"[CLIENT] Habilidad seleccionada: {player.abilities[abilityIndex].abilityName}");

        // iniciar selección de objetivo
        TargetSelectionController.Instance
            .BeginTargetSelection(this, abilityIndex);
    }

    // (para más adelante, ítems)
    public void SendItemChoice(int itemIndex)
    {
        if (!IsOwner) return;
        if (!player.CanAct()) return;

        Debug.Log($"[CLIENT] Item seleccionado índice {itemIndex}");

        // aquí luego hacemos TargetSelection también
    }

    // ================== DESDE TARGET SELECTION ==================

    public void SubmitAbilityWithTarget(int abilityIndex, EnemyCharacter enemy)
    {
        if (!IsOwner) return;
        if (!player.CanAct()) return;

        var targetNetObj = enemy.GetComponent<NetworkObject>();
        if (targetNetObj == null) return;

        selectedTargetId = targetNetObj.NetworkObjectId;

        Debug.Log($"[CLIENT] Enviando acción al servidor: habilidad {abilityIndex} → {enemy.name}");

        // enviar al servidor
        NetworkBattleManager.Instance
            .SubmitPlayerActionServerRpc(abilityIndex, selectedTargetId);

        // bloquear input local
        player.EnableInput(false);

        // limpiar selección local
        selectedAbilityIndex = -1;
        selectedTargetId = 0;
    }
}
