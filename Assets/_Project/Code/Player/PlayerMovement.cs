using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls player movement across the board graph.
///
/// Responsibilities:
/// - Track the player's current board location
/// - Move the player between connected board spaces.
/// - Pause movement at intersections
/// - Store remaining movement after route selection
/// - Resume movement after a route is confirmed.
/// - Maintain movement state information.
///
/// This script does NOT determine whose turn it is.
/// TurnManager controls turn order and instructs the player when movement should begin.
/// </summary>

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private BoardSpace currentSpace; // The board node currently occupied by the player.
    [SerializeField] private MovementState movementState = MovementState.Idle; // Tracks the player's current Movement State.

    private int remainingSpaces; // Number of spaces remaining from the current dice roll.
    // This value decreases as the player moves across the board.
    
    private BoardSpace pendingIntersection; // Stores the intersection currently awaiting a route choice.

    private PathDirection selectedDirection; // Route currently selected by player.
    private bool waitingForConfirmation; // Tracks whether a route has been selected and is waiting for player confirmation.

    public bool IsWaitingForDirection()
    {
        // Returns true when the player has reached an intersection and is waiting for a route choice.
        return movementState == MovementState.WaitingForDirection;
    }
    
    public BoardSpace CurrentSpace
    {
        // Gets or sets the board space currently occupied by the player.
        get => currentSpace;
        set => currentSpace = value;
    }

    public MovementState CurrentMovementState => movementState; // Provides read-only access to the player's current movement state.

    private void Start()
    {
        // Places the player on their assigned starting board space when the scene begins.
        
        if (currentSpace != null)
        {
            transform.position = currentSpace.transform.position;
        }
    }

    // Handles intersection input while waiting for route selection.
    // Route selection logic is gradually being moved into Turn Manager. This method may be removed during refactoring.
    private void Update()
    {
        if (movementState != MovementState.WaitingForDirection)
            return;

        HandleIntersectionInput();
    }

    // Begins a new movement sequence after a dice-block roll.
    public void StartMovingSpaces(int spaces)
    {
        movementState = MovementState.Moving;

        remainingSpaces = spaces;

        ContinueMovement();
    }

    // continues movement along the board graph until:
    // - movement is exhausted.
    // - an intersection is reached.
    // - A dead end is encountered.
    public void ContinueMovement()
    {
        BoardSpace targetSpace = currentSpace;

        while (remainingSpaces > 0)
        {
            if (targetSpace.NextSpaces.Count == 0)
            {
                Debug.LogWarning(
                    $"Space {targetSpace.SpaceIndex} has no Next Spaces assigned");

                break;
            }
            
            targetSpace = targetSpace.NextSpaces[0]; // move to the next connected node

            remainingSpaces--;

            currentSpace = targetSpace;

            transform.position =
                currentSpace.transform.position;

            Debug.Log(
                $"Moved to Space {currentSpace.SpaceIndex}");

            // Stop movement when an intersection is reached.
            if (currentSpace.SpaceType ==
                SpaceType.Intersection)
            {
                pendingIntersection = currentSpace;

                movementState =
                    MovementState.WaitingForDirection;

                Debug.Log(
                    $"Reached Intersection {currentSpace.SpaceIndex}");

                Debug.Log(
                    $"Remaining Spaces: {remainingSpaces}");

                return;
            }
        }

        // movement has completed successfully.
        movementState = MovementState.Idle;
    }

    // Temporary keyboard-based route selection system.
    // May be replaced by Input Actions in a future update.
    private void HandleIntersectionInput()
    {
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            selectedDirection =
                pendingIntersection.OptionOneDirection;

            Debug.Log(
                $"Selected {selectedDirection}");

            waitingForConfirmation = true;
        }

        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            selectedDirection =
                pendingIntersection.OptionTwoDirection;

            Debug.Log(
                $"Selected {selectedDirection}");

            waitingForConfirmation = true;
        }

        if (waitingForConfirmation &&
            Keyboard.current.enterKey.wasPressedThisFrame)
        {
            ConfirmDirection();
        }
    }

    // Stores the player's chosen direction
    public void SelectDirection(PathDirection direction)
    {
        selectedDirection = direction;
        
        Debug.Log($"Selected Direction: {selectedDirection}");
    }
    
    // Checks whether the specified direction is available at the current intersection
    public bool IsDirectionAvailable(
        PathDirection direction)
    {
        if (pendingIntersection == null)
            return false;

        return direction ==
               pendingIntersection.OptionOneDirection
               ||
               direction ==
               pendingIntersection.OptionTwoDirection;
    }
    
    
    // Applies the selected route and resumes movement.
    public void ConfirmDirection()
    {
        Debug.Log(
            $"Confirmed {selectedDirection}");

        if (selectedDirection ==
            pendingIntersection.OptionOneDirection)
        {
            currentSpace =
                pendingIntersection.NextSpaces[0];
        }
        else
        {
            currentSpace =
                pendingIntersection.NextSpaces[1];
        }

        transform.position =
            currentSpace.transform.position;

        movementState =
            MovementState.Moving;

        waitingForConfirmation = false;

        ContinueMovement();
    }
}
