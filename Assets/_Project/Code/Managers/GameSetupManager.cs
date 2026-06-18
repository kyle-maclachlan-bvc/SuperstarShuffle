using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameSetupManager : MonoBehaviour
{
    [SerializeField] private TurnManager turnManager;

    [Header("Setup Progress")]
    [SerializeField] private int currentSetupStep = 0;

    [Header("Player Rolls")]
    [SerializeField] private int PlayerOneRoll;
    [SerializeField] private int PlayerTwoRoll;
    [SerializeField] private int PlayerThreeRoll;
    [SerializeField] private int PlayerFourRoll;

    private List<int> availableRolls = new();
    
    private bool playerOneRolled;
    private bool playerTwoRolled;
    private bool playerThreeRolled;
    private bool playerFourRolled;

    private bool turnOrderDetermined;
    private bool waitingForTurnOrder;

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameState.GameSetup)
            return;

        if (waitingForTurnOrder)
            HandleTurnOrderInput();

        if (InputManager.Instance.Controls.GameSetup.Confirm.WasPressedThisFrame())
        {
            if (currentSetupStep == 2 && !turnOrderDetermined)
            {
                Debug.Log("All players must roll first");
                return;
            }

            AdvanceSetup();
        }
    }

    private void AdvanceSetup()
    {
        switch (currentSetupStep)
        {
            case 0:
                BeginBoardIntroduction();
                break;
            case 1:
                DetermineTurnOrder();
                break;
            case 2:
                GiveStartingCoins();
                break;
            case 3:
                BeginGameplay();
                break;
        }
        
        currentSetupStep++;
    }
    
    public void StartGameSetup()
    {
        Debug.Log("Press Confirm to Continue");
    }

    private void BeginBoardIntroduction()
    {
        Debug.Log("Welcome to Glitterdeep Mines!");
    }

    private void DetermineTurnOrder()
    {
        Debug.Log("DetermineTurnOrder");

        availableRolls.Clear();
        for (int i = 1; i <= 10; i++)
            availableRolls.Add(i);

        playerOneRolled = false;
        playerTwoRolled = false;
        playerThreeRolled = false;
        playerFourRolled = false;
        
        turnOrderDetermined = false;
        
        waitingForTurnOrder = true;
    }

    private int RollUniqueNumber()
    {
        int randomIndex = Random.Range(0, availableRolls.Count);
        int roll = availableRolls[randomIndex];
        availableRolls.RemoveAt(randomIndex);
        return roll;
    }

    private void HandleTurnOrderInput()
    {
        Player player1 = GameManager.Instance.Players[0];
        Player player2 = GameManager.Instance.Players[1];
        Player player3 = GameManager.Instance.Players[2];
        Player player4 = GameManager.Instance.Players[3];
        
        if (!playerOneRolled && InputManager.Instance.PlayerRollPressed(1))
        {
            PlayerOneRoll = RollUniqueNumber();
            playerOneRolled = true;
            
            Debug.Log($"Player 1 Rolled {PlayerOneRoll}");
        }
        
        if (!playerTwoRolled && InputManager.Instance.PlayerRollPressed(2))
        {
            PlayerTwoRoll = RollUniqueNumber();
            playerTwoRolled = true;
            
            Debug.Log($"Player 2 Rolled {PlayerTwoRoll}");
        }
        
        if (!playerThreeRolled && InputManager.Instance.PlayerRollPressed(3))
        {
            PlayerThreeRoll = RollUniqueNumber();
            playerThreeRolled = true;
            
            Debug.Log($"Player 3 Rolled {PlayerThreeRoll}");
        }
        
        if (!playerFourRolled && InputManager.Instance.PlayerRollPressed(4))
        {
            PlayerFourRoll = RollUniqueNumber();
            playerFourRolled = true;
            
            Debug.Log($"Player 4 Rolled {PlayerFourRoll}");
        }

        if (!turnOrderDetermined && playerOneRolled && playerTwoRolled && playerThreeRolled && playerFourRolled)
        {
            turnOrderDetermined = true;
            waitingForTurnOrder = false;

            BuildTurnOrder();
            //Debug.Log("All players have rolled");
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

    private void GiveStartingCoins()
    {
        Debug.Log("Give Starting 10 Coins");

        foreach (Player player in GameManager.Instance.Players)
            player.Currency.AddCoins(10);
    }

    private void BeginGameplay()
    {
        Debug.Log("Begin Gameplay");
        GameManager.Instance.ChangeGameState(GameState.PlayerTurn);
    }
}
