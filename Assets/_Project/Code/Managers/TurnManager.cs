using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class TurnManager : MonoBehaviour
{
    [SerializeField] private DiceBlock diceBlock; // Generates movement value used for board traversal
    [SerializeField] private CameraController cameraController; // Controls which player the camera follows.
    [SerializeField] private List<Player> players;
    
    private int currentPlayerIndex = 0; // Index of the player whose turn is currently active.

    private void OnEnable()
    {
        GameEvents.OnGameStateChange += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStateChange -= HandleGameStateChanged;
    }
    
    // Processes turn-related input each frame
    // Responsibilities:
        // Detect die rolls
        // Handle intersection selections
        // ignore input from inactive players
    private void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameState.PlayerTurn)
            return;
        
        Player currentPlayer = players[currentPlayerIndex];
        
        if (currentPlayer.TurnState.CurrentState == PlayerState.Waiting ||
            currentPlayer.TurnState.CurrentState == PlayerState.Disabled)
            return;

        PlayerMovement movement = currentPlayer.Movement;
        PlayerController controller = currentPlayer.Controller;
        
        /*// only process input for the active player.
        if (playerStates[currentPlayerIndex].CurrentState
            == PlayerState.Waiting || playerStates[currentPlayerIndex].CurrentState == PlayerState.Disabled)
            return;
        
        PlayerMovement currentPlayer = players[currentPlayerIndex];
        PlayerController currentController = playerControllers[currentPlayerIndex];
        */


        // if movement is paused at an intersection, process route selection input
        if (movement.IsWaitingForDirection())
        {
            HandleIntersectionInput(movement, controller);
            return;
        }
        
        // detect dice roll input.
        if (currentPlayer.TurnState.CurrentState == PlayerState.TakingTurn &&
            controller.RollPressed())
        {
            RollDice();
        }
    }



    public void BeginTurn()
    {
        foreach (Player player in players)
        {
            player.TurnState.CurrentState = PlayerState.Waiting;

            player.Controller.DisableControls();
        }
        
        Player currentPlayer = players[currentPlayerIndex];
        currentPlayer.TurnState.CurrentState = PlayerState.TakingTurn;
        currentPlayer.Controller.EnableControls();
        cameraController.FocusPlayer(currentPlayer.transform);
        
        //Debug.Log($"{currentPlayer.name} Turn Started.");

        /*Debug.Log($"Player {currentPlayerIndex + 1} Turn Started");

        for (int i = 0; i < playerStates.Count; i++)
        {
            playerStates[i].CurrentState = PlayerState.Waiting;

            playerControllers[i].DisableControls();
        }

        playerStates[currentPlayerIndex].CurrentState = PlayerState.TakingTurn;
        playerControllers[currentPlayerIndex].EnableControls();

        cameraController.FocusPlayer(players[currentPlayerIndex].transform);
        */
    }

        // Rolls the dice and begins player movement
        // The active player transitions from TakingTurn to Moving
        void RollDice()
        {
            Player currentPlayer = players[currentPlayerIndex];

            if (currentPlayer.TurnState.CurrentState != PlayerState.TakingTurn)
                return;

            int rollValue = diceBlock.Roll();

            Debug.Log($"Rolled: {rollValue}");

            currentPlayer.TurnState.CurrentState = PlayerState.Moving;

            currentPlayer.Movement.StartMovingSpaces(rollValue);
        }

        // Processes player input while waiting at an intersection.
        // allows players to:
            // select a route
            // confirm a route
            // resume movement
        private void HandleIntersectionInput(PlayerMovement player, PlayerController controller)
        {
            Vector2 routeInput = controller.RouteSelection();

            if (routeInput.x > 0.5f && player.IsDirectionAvailable(PathDirection.Right))
                player.SelectDirection(PathDirection.Right);
            
            if (routeInput.x < -0.5 && player.IsDirectionAvailable(PathDirection.Left))
                player.SelectDirection(PathDirection.Left);
            
            if (routeInput.y > 0.5f && player.IsDirectionAvailable(PathDirection.Up))
                player.SelectDirection(PathDirection.Up);
            
            if (routeInput.y < -0.5 && player.IsDirectionAvailable(PathDirection.Down))
                player.SelectDirection(PathDirection.Down);

            if (controller.ConfirmPressed())
                player.ConfirmDirection();

            
        }
        
// Ends the current player's turn and advances to the next player.
        // If the final player has completed their turn, GameManager is notified that the round has ended.
        private void EndPlayerTurn()
        {
            players[currentPlayerIndex].TurnState.CurrentState = PlayerState.Waiting;

            Player currentPlayer = players[currentPlayerIndex];
            Debug.Log($"{currentPlayer.name} finished their turn with {currentPlayer.Currency.Coins} coins");
            
            currentPlayerIndex++;

            if (currentPlayerIndex >= players.Count)
            {
                currentPlayerIndex = 0;
                
                foreach(Player player in players)
                    player.Controller.DisableControls();

                GameManager.Instance.RoundCompleted();

                return;
            }

            BeginTurn();
        }

        private void HandleGameStateChanged(GameState newState)
        {
            Debug.Log($"HandleGameStateChanged Fired: {newState}");
            
            if (newState != GameState.PlayerTurn)
                return;
            
            Debug.Log($"Current Turn = {GameManager.Instance.CurrentTurn}");


            players = GameManager.Instance.TurnOrder;

            foreach (Player player in players)
            {
                Debug.Log($"Player Reference = {player}");

                if (player == null)
                {
                    Debug.LogError("PLAYER IS NULL");
                    continue;
                }

                Debug.Log($"Movement = {player.Movement}");
                Debug.Log($"Controller = {player.Controller}");
                Debug.Log($"TurnState = {player.TurnState}");

                if (player.Movement == null)
                {
                    Debug.LogError($"{player.name} MOVEMENT IS NULL");
                    continue;
                }

                player.Movement.OnMovementFinished -= EndPlayerTurn;
                player.Movement.OnMovementFinished += EndPlayerTurn;
            }

            currentPlayerIndex = 0;

            BeginTurn();

            //Debug.Log("TurnManager Loaded Turn Order");
            //Debug.Log($"Game State Changed: {GameManager.Instance.CurrentGameState}");
        }
    
}
