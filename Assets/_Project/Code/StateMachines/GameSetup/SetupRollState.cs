using System.Collections.Generic;
using UnityEngine;

public class SetupRollState : SetupState
{
    private List<int> availableRolls = new();

    private int PlayerOneRoll;
    private int PlayerTwoRoll;
    private int PlayerThreeRoll;
    private int PlayerFourRoll;

    private bool playerOneRolled;
    private bool playerTwoRolled;
    private bool playerThreeRolled;
    private bool playerFourRolled;

    private bool turnOrderDetermined;
    
    public SetupRollState(GameSetupManager setup) : base(setup)
    {
    }

    public override void Enter()
    {
        //Debug.Log("DetermineTurnOrder");
        
        InputManager.Instance.Controls.BoardGame.Roll.performed +=  ctx => HandleTurnOrderInput(ctx.ReadValue<int>());

        
        availableRolls.Clear();
        
        for (int i = 1; i <= 10; i++)
            availableRolls.Add(i);

        playerOneRolled = false;
        playerTwoRolled = false;
        playerThreeRolled = false;
        playerFourRolled = false;
        
        turnOrderDetermined = false;
    }
    
    private int RollUniqueNumber()
    {
        int randomIndex = Random.Range(0, availableRolls.Count);
        int roll = availableRolls[randomIndex];
        availableRolls.RemoveAt(randomIndex);
        return roll;
    }

    private void HandleTurnOrderInput(int playerID)
    {
        Player player1 = GameManager.Instance.Players[0];
        Player player2 = GameManager.Instance.Players[1];
        Player player3 = GameManager.Instance.Players[2];
        Player player4 = GameManager.Instance.Players[3];

        switch (playerID)
        {
            case 1:
                PlayerOneRoll = RollUniqueNumber();
                GameEvents.OnDiceRolled?.Invoke(player1, PlayerOneRoll);
                playerOneRolled = true;
                Debug.Log($"Player 1 Rolled {PlayerOneRoll}");
                break;
            case 2:
                PlayerTwoRoll = RollUniqueNumber();
                GameEvents.OnDiceRolled?.Invoke(player2, PlayerTwoRoll);
                playerTwoRolled = true;
                Debug.Log($"Player 2 Rolled {PlayerTwoRoll}");
                break;
            case 3:
                PlayerThreeRoll = RollUniqueNumber();
                GameEvents.OnDiceRolled?.Invoke(player3, PlayerThreeRoll);
                playerThreeRolled = true;
                Debug.Log($"Player 3 Rolled {PlayerThreeRoll}");
                break;
            case 4:
                PlayerFourRoll = RollUniqueNumber();
                GameEvents.OnDiceRolled?.Invoke(player4, PlayerFourRoll);
                playerFourRolled = true;
                Debug.Log($"Player 4 Rolled {PlayerFourRoll}");
                break;
        }

        if (!turnOrderDetermined && playerOneRolled && playerTwoRolled && playerThreeRolled && playerFourRolled)
        {
            turnOrderDetermined = true;

            BuildTurnOrder();
            setup.EnterPresentationState();
        }
    }
    
    private void BuildTurnOrder()
    {
        List<(Player player, int roll)> rollResults = new()
        {
            (GameManager.Instance.Players[0], PlayerOneRoll),
            (GameManager.Instance.Players[1], PlayerTwoRoll),
            (GameManager.Instance.Players[2], PlayerThreeRoll),
            (GameManager.Instance.Players[3], PlayerFourRoll)
        };

        rollResults.Sort((a, b) => b.roll.CompareTo(a.roll));

        GameManager.Instance.TurnOrder.Clear();

        for (int i = 0; i < rollResults.Count; i++)
        {
            Player player = rollResults[i].player;

            GameManager.Instance.TurnOrder.Add(player);

            // Persist turn order into PlayerData
            player.Data.TurnOrderPosition = i;
        }

        Debug.Log("Final Turn Order:");

        for (int i = 0; i < GameManager.Instance.TurnOrder.Count; i++)
        {
            Debug.Log(
                $"{i + 1}: {GameManager.Instance.TurnOrder[i].name} " +
                $"(Position {GameManager.Instance.TurnOrder[i].Data.TurnOrderPosition})"
            );
        }
    }
}
