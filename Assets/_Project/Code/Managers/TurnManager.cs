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
    //[SerializeField] private List<PlayerMovement> players; // Collection of player movement components. Used to move the players across the board.
    //[SerializeField] private List<PlayerTurnState> playerStates; // Collection of player state components. Used to determine whose turn is active.
    //[SerializeField] private List<PlayerController> playerControllers; // Collection of player controller components. Used to enable and disable player input.
    [SerializeField] private CameraController cameraController; // Controls which player the camera follows.
    [SerializeField] private List<Player> players;
    
    private int currentPlayerIndex = 0; // Index of the player whose turn is currently active.

    private void Start()
    {
        //Debug.Log("TurnManager Waiting for GameSetup");
        
        //foreach (PlayerMovement player in players)
            //player.OnMovementFinished += EndPlayerTurn;
    }

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
        if (controller.RollPressed())
        {
            RollDice();
        }
    }

    // Starts the current player's turn.
    // Responsibilities:
        // reset all player states
        // disable inactive player controls
        // enable the active player's controls
        // focus the camera on the active player.
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
            int rollValue = diceBlock.Roll();

            Debug.Log($"Rolled: {rollValue}");

            players[currentPlayerIndex].TurnState.CurrentState = PlayerState.Moving;

            players[currentPlayerIndex].Movement.StartMovingSpaces(rollValue);
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
            players[currentPlayerIndex].TurnState.CurrentState =
                PlayerState.Waiting;

            Player currentPlayer = players[currentPlayerIndex];
            Debug.Log($"{currentPlayer.name} finished their turn with {currentPlayer.Currency.Coins} coins");
            
            currentPlayerIndex++;

            if (currentPlayerIndex >= players.Count)
            {
                currentPlayerIndex = 0;

                GameManager.Instance.RoundCompleted();
            }

            BeginTurn();
        }

        private void HandleGameStateChanged(GameState newState)
        {
            if (newState == GameState.PlayerTurn)
                BeginTurn();

            players = GameManager.Instance.TurnOrder;

            foreach (Player player in players)
            {
                player.Movement.OnMovementFinished -= EndPlayerTurn;
                player.Movement.OnMovementFinished += EndPlayerTurn;
            }

            currentPlayerIndex = 0;

            BeginTurn();
            
           //Debug.Log("TurnManager Loaded Turn Order");
            //Debug.Log($"Game State Changed: {GameManager.Instance.CurrentGameState}");
        }
}
