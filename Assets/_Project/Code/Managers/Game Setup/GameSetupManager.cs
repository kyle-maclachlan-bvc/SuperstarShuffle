using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameSetupManager : MonoBehaviour
{
    [SerializeField] private TurnManager turnManager;

    [Header("Setup Progress")]
    //[SerializeField] private int currentSetupStep = 0;

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
    private bool waitingForCoinPresentation;
    private bool waitingForFinalDialogue;

    private void OnEnable()
    {
        GameEvents.OnDialogueFinished += HandleDialogueFinished;
        GameEvents.OnTurnOrderPresentationFinished += PresentStartingCoins;
        GameEvents.OnStartingCoinsPresentationFinished += HandleCoinsFinished;
    }

    private void OnDisable()
    {
        GameEvents.OnDialogueFinished -= HandleDialogueFinished;
        GameEvents.OnTurnOrderPresentationFinished -= PresentStartingCoins;
        GameEvents.OnStartingCoinsPresentationFinished -=  HandleCoinsFinished;
    }
    
    private void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameState.GameSetup)
            return;

        if (waitingForTurnOrder)
            HandleTurnOrderInput();
    }
    private void HandleDialogueFinished()
    {
        // Initial introduction finished.
        if (!turnOrderDetermined)
        {
            DetermineTurnOrder();
            return;
        }

        // "Take these 10 coins." dialogue acknowledged.
        if (waitingForCoinPresentation)
        {
            waitingForCoinPresentation = false;

            GameEvents.OnStartingCoinsPresentationRequested?.Invoke();
            return;
        }

        // Final "Good luck" dialogue acknowledged.
        if (waitingForFinalDialogue)
        {
            waitingForFinalDialogue = false;

            BeginGameplay();
        }
    }
    
    public void StartGameSetup()
    {
        foreach (Player player in GameManager.Instance.Players)
        {
            player.Data.CurrentSpaceIndex = player.Data.StartingSpaceIndex;
        }
        
        GameEvents.OnGameSetupStarted?.Invoke();
        
        GameEvents.OnDialogueRequested?.Invoke("Mine Foreman", new string[]
        {
            "Welcome to Glitterdeep Mines!",
            "Deep beneath the mountain lies one of the oldest mining operations, famous for its glittering crystal caverns and sprawling network of tunnels.",
            "For generations, miners searched these caves for rare gems and precious ores.",
            "Although much of the mine has been abandoned, valuable discovers can still be found throughout its twisting passages.",
            "Before the expedition begins, we'll determine the order in which everyone takes their turns."
        });
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
            GameEvents.OnDiceRolled?.Invoke(player1, PlayerOneRoll);
            playerOneRolled = true;
            
            Debug.Log($"Player 1 Rolled {PlayerOneRoll}");
        }
        
        if (!playerTwoRolled && InputManager.Instance.PlayerRollPressed(2))
        {
            PlayerTwoRoll = RollUniqueNumber();
            GameEvents.OnDiceRolled?.Invoke(player2, PlayerTwoRoll);
            playerTwoRolled = true;
            
            Debug.Log($"Player 2 Rolled {PlayerTwoRoll}");
        }
        
        if (!playerThreeRolled && InputManager.Instance.PlayerRollPressed(3))
        {
            PlayerThreeRoll = RollUniqueNumber();
            GameEvents.OnDiceRolled?.Invoke(player3, PlayerThreeRoll);
            playerThreeRolled = true;
            
            Debug.Log($"Player 3 Rolled {PlayerThreeRoll}");
        }
        
        if (!playerFourRolled && InputManager.Instance.PlayerRollPressed(4))
        {
            PlayerFourRoll = RollUniqueNumber();
            GameEvents.OnDiceRolled?.Invoke(player4, PlayerFourRoll);
            playerFourRolled = true;
            
            Debug.Log($"Player 4 Rolled {PlayerFourRoll}");
        }

        if (!turnOrderDetermined && playerOneRolled && playerTwoRolled && playerThreeRolled && playerFourRolled)
        {
            turnOrderDetermined = true;
            waitingForTurnOrder = false;

            BuildTurnOrder();
            GameEvents.OnTurnOrderPresentationRequested?.Invoke();
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

    private void PresentStartingCoins()
    {
        waitingForCoinPresentation = true;

        GameEvents.OnDialogueRequested?.Invoke(
            "Mine Foreman",
            new string[]
            {
                "Every explorer needs a little spending money before heading into the mines. Take these 10 coins."
            });
    }

    private void HandleCoinsFinished()
    {
        waitingForFinalDialogue = true;

        GameEvents.OnDialogueRequested?.Invoke(
            "Mine Foreman",
            new string[]
            {
                "Good luck in Glitterdeep Mines!"
            });
    }

    private void BeginGameplay()
    {
        Debug.Log("Begin Gameplay");
        GameEvents.OnGameStateRequested?.Invoke(GameState.PlayerTurn);
    }
}
