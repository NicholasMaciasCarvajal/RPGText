using Unity.Netcode;
using UnityEngine;

public class PlayerTurnController : NetworkBehaviour
{
    private PlayerCharacter player;
    private bool actionSubmitted = false;

    private NetworkBattleManager battleManager;

    private void Awake()
    {
        player = GetComponent<PlayerCharacter>();
    }

    private void Start()
    {
        battleManager = FindFirstObjectByType<NetworkBattleManager>();
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (!player.CanAct()) return;
        if (actionSubmitted) return;

        // Ejemplo simple: teclas directas
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SubmitAbility(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SubmitItem(0);
    }

    private void SubmitAbility(int abilityIndex)
    {
        if (battleManager == null) return;

        // aquí elegirías también el objetivo
        ulong dummyTarget = 0;

        battleManager.SubmitPlayerActionServerRpc(
            abilityIndex,
            dummyTarget
        );

        actionSubmitted = true;
        player.EnableInput(false);
    }

    private void SubmitItem(int itemIndex)
    {
        if (battleManager == null) return;

        ulong dummyTarget = 0;

        battleManager.SubmitPlayerActionServerRpc(
            itemIndex,
            dummyTarget
        );

        actionSubmitted = true;
        player.EnableInput(false);
    }

    public void ResetTurnInput()
    {
        actionSubmitted = false;
        player.EnableInput(true);
    }

    public void SendAbilityChoice(int abilityIndex)
    {
        // entra a modo seleccionar objetivo
        TargetSelectionController.Instance.BeginTargetSelection(this, abilityIndex);
    }

    public void SendItemChoice(int itemIndex)
    {
        TargetSelectionController.Instance.BeginTargetSelection(this, itemIndex);
    }

    public void SubmitAbilityWithTarget(int abilityIndex, EnemyCharacter target)
    {
        var targetNetwork = target.GetComponent<NetworkObject>();

        battleManager.SubmitPlayerActionServerRpc(
            abilityIndex,
            targetNetwork.NetworkObjectId
        );

        actionSubmitted = true;
        player.EnableInput(false);
    }

}
