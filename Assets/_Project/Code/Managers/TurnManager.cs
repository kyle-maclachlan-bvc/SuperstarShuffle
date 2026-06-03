using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Controls player turn order during board gameplay
///
/// Responsibilities:
/// - Track the active player.
/// Start and end player turns
/// Enable and disable player controls
/// Handle dice rolling
/// Process intersection selections
/// Advance to the next player
/// Notify GameManager when a round is complete.
///
/// TurnManager manages the flow of turns between players, but does not control the overall match.
/// GameManager remains responsible for high level game progression
/// </summary>

public class TurnManager : MonoBehaviour
{
    [SerializeField] private DiceBlock diceBlock; // Generates movement value used for board traversal
    [SerializeField] private List<PlayerMovement> playerMovements; // Collection of player movement components. Used to move the players across the board.
    [SerializeField] private List<PlayerTurnState> playerStates; // Collection of player state components. Used to determine whose turn is active.
    [SerializeField] private List<PlayerController> playerControllers; // Collection of player controller components. Used to enable and disable player input.
    [SerializeField] private CameraController cameraController; // Controls which player the camera follows.
    
    private int currentPlayerIndex = 0; // Index of the player whose turn is currently active.
    [SerializeField] private List<Player> players;
    
    private void Start()
    {
        Debug.Log("TurnManager Start");
        
        players = GameManager.Instance.TurnOrder;
        
        foreach (Player player in players)
        {
            Debug.Log($"Subscribing {player.name}");

            Debug.Log($"Movement = {player.Movement}");
            Debug.Log($"Controller = {player.Controller}");
            Debug.Log($"TurnState = {player.TurnState}");

            player.Movement.OnMovementFinished += EndPlayerTurn;

            Debug.Log($"Finished {player.name}");
        }
        
        Debug.Log("Finished Subscribing");

        BeginTurn();
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
        
        /*
        // only process input for the active player.
        if (playerStates[currentPlayerIndex].CurrentState
            == PlayerState.Waiting || playerStates[currentPlayerIndex].CurrentState == PlayerState.Disabled)
            return;
        
        PlayerMovement currentPlayer = players[currentPlayerIndex];
        PlayerController currentController = playerControllers[currentPlayerIndex];
        */

        if (movement.IsWaitingForDirection())
        {
            HandleIntersectionInput(movement, controller);
            return;
        }

        if (controller.RollPressed())
            RollDice();

        /*
        // if movement is paused at an intersection, process route selection input
        if (currentPlayer.IsWaitingForDirection())
        {
            HandleIntersectionInput(
                currentPlayer, currentController);

            return;
        }

        // detect dice roll input.
        if (playerControllers[currentPlayerIndex].RollPressed())
        {
            RollDice();
        }
        */
    }

    // Starts the current player's turn.
    // Responsibilities:
        // reset all player states
        // disable inactive player controls
        // enable the active player's controls
        // focus the camera on the active player.
    private void BeginTurn()
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

        /*

        //Debug.Log("BeginTurn Called");

            for (int i = 0; i < playerStates.Count; i++)
            {
                playerStates[i].CurrentState =
                    PlayerState.Waiting;

                playerControllers[i].DisableControls();
            }

            playerStates[currentPlayerIndex].CurrentState =
                PlayerState.TakingTurn;

            playerControllers[currentPlayerIndex].EnableControls();

            cameraController.FocusPlayer(
                players[currentPlayerIndex].transform);

            //Debug.Log($"Player {currentPlayerIndex + 1} Turn Started");

            */
    }

        // Rolls the dice and begins player movement
        // The active player transitions from TakingTurn to Moving
        void RollDice()
        {
            int rollValue = diceBlock.Roll();
            
            players[currentPlayerIndex].TurnState.CurrentState = PlayerState.Moving;
            players[currentPlayerIndex].Movement.StartMovingSpaces(rollValue);
           
            /*Debug.Log($"Rolled: {rollValue}");

            playerStates[currentPlayerIndex].CurrentState = PlayerState.Moving;

            players[currentPlayerIndex].StartMovingSpaces(rollValue);*/
        }

        // Processes player input while waiting at an intersection.
        // allows players to:
            // select a route
            // confirm a route
            // resume movement
        private void HandleIntersectionInput(PlayerMovement player, PlayerController controller)
        {
            if (controller.SelectRightPressed()
                &&
                player.IsDirectionAvailable(
                    PathDirection.Right))
            {
                player.SelectDirection(
                    PathDirection.Right);
            }

            if (controller.SelectDownPressed()
                &&
                player.IsDirectionAvailable(
                    PathDirection.Down))
            {
                player.SelectDirection(
                    PathDirection.Down);
            }

            if (controller.SelectLeftPressed()
                &&
                player.IsDirectionAvailable(
                    PathDirection.Left))
            {
                player.SelectDirection(
                    PathDirection.Left);
            }

            if (controller.SelectUpPressed()
                &&
                player.IsDirectionAvailable(
                    PathDirection.Up))
            {
                player.SelectDirection(
                    PathDirection.Up);
            }

            // apply the selected route and continue movement
            if (controller.ConfirmPressed())
            {
                player.ConfirmDirection();
            }
        }
        
// Ends the current player's turn and advances to the next player.
        // If the final player has completed their turn, GameManager is notified that the round has ended.
        private void EndPlayerTurn()
        {
            players[currentPlayerIndex].TurnState.CurrentState = PlayerState.Waiting;
            //playerStates[currentPlayerIndex].CurrentState = PlayerState.Waiting;

            currentPlayerIndex++;

            if (currentPlayerIndex >= players.Count)
            {
                currentPlayerIndex = 0;

                GameManager.Instance.RoundCompleted();
            }

            BeginTurn();
            
        }
}
