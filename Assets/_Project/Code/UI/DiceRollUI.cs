using TMPro;
using UnityEngine;

public class DiceRollUI : MonoBehaviour
{
    [SerializeField] private TMP_Text rollText;
    [SerializeField] private Canvas canvas;

    private Camera mainCamera;
    private Player player;

    private void Awake()
    {
        mainCamera = Camera.main;
        player = GetComponentInParent<Player>();
        canvas.enabled = false;
    }

    private void OnEnable()
    {
        GameEvents.OnDiceRolled += HandleDiceRoll;
        GameEvents.OnTurnOrderConfirmed += HandleTurnOrderConfirmed;
        GameEvents.OnMovementStepCompleted += HandleMovementStep;
        GameEvents.OnMovementFinished += HandleMovementFinished;
    }

    private void OnDisable()
    {
        GameEvents.OnDiceRolled -= HandleDiceRoll;
        GameEvents.OnTurnOrderConfirmed -= HandleTurnOrderConfirmed;
        GameEvents.OnMovementStepCompleted -= HandleMovementStep;
        GameEvents.OnMovementFinished -= HandleMovementFinished;
    }

    private void LateUpdate()
    {
        if (!canvas.enabled)
            return;

        canvas.transform.forward = mainCamera.transform.forward;
    }

    private void HandleDiceRoll(Player rolledPlayer, int value)
    {
        if (rolledPlayer != player)
            return;
        
        ShowRoll(value);
    }

    private void HandleMovementStep(Player movedPlayer, int remainingSpaces)
    {
        if (movedPlayer != player)
            return;
        
        rollText.text = remainingSpaces.ToString();
    }

    private void HandleMovementFinished(Player movedPlayer)
    {
        if (movedPlayer != player)
            return;
        
        canvas.enabled = false;
    }

    private void HandleTurnOrderConfirmed(Player confirmedPlayer)
    {
        if (confirmedPlayer != player)
            return;

        canvas.enabled = false;
    }

    public void ShowRoll(int value)
    {
        rollText.text = value.ToString();
        canvas.enabled = true;
    }


}
