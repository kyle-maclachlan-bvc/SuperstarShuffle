using System;
using System.Collections;
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
    [SerializeField] private float moveSpeed = 5f; // movement speed for the Coroutine to move players between spaces.
    
    private int remainingSpaces; // Number of spaces remaining from the current dice roll.
    // This value decreases as the player moves across the board.
    
    private BoardSpace intersectionSpace; // Stores the intersection currently awaiting a route choice.
    
    private PathDirection selectedDirection; // Route currently selected by player.
    private bool waitingForConfirmation; // Tracks whether a route has been selected and is waiting for player confirmation.

    public Action OnMovementFinished;
    
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

        StartCoroutine(ContinueMovement());
    }

    // continues movement along the board graph until:
    // - movement is exhausted.
    // - an intersection is reached.
    // - A dead end is encountered.
    private IEnumerator ContinueMovement()
    {
        while (remainingSpaces > 0)
        {
            if (currentSpace.NextSpaces.Count == 0)
            {
                Debug.LogWarning($"Space {currentSpace.SpaceIndex} has no Next Space assigned");
                break;
            }


            BoardSpace destinationSpace = currentSpace.NextSpaces[0];
            yield return StartCoroutine(MoveToSpace(destinationSpace));

            currentSpace = destinationSpace;
            
            if (currentSpace.SpaceType != SpaceType.Transit && currentSpace.SpaceType != SpaceType.Intersection)
            {
                remainingSpaces--;
            }

            Debug.Log($"Space: {currentSpace.SpaceIndex} | Type: {currentSpace.SpaceType} | Remaining: {remainingSpaces}");

            if (currentSpace.SpaceType == SpaceType.Intersection)
            {
                intersectionSpace = currentSpace;
                movementState = MovementState.WaitingForDirection;

                //Debug.Log($"Reached Intersection {currentSpace.SpaceIndex}");
                //Debug.Log($"Remaining Spaces: {remainingSpaces}");

                yield break;
            }

            if (currentSpace.SpaceType == SpaceType.Transit)
            {
                Debug.Log($"Reached Transit Node {currentSpace.SpaceIndex}");
                continue;
            }
            
        }

        movementState = MovementState.Idle;
        OnMovementFinished?.Invoke();
    }

    private IEnumerator MoveToSpace(BoardSpace targetSpace)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = targetSpace.transform.position;
        
        float journeyLength = Vector3.Distance(startPosition, targetPosition);

        float elapsedTime = 0f;

        while (elapsedTime < journeyLength / moveSpeed)
        {
            elapsedTime += Time.deltaTime;
            
            float t = elapsedTime / (journeyLength / moveSpeed);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        transform.position = targetPosition;
    }

    // Temporary keyboard-based route selection system.
    // May be replaced by Input Actions in a future update.
    private void HandleIntersectionInput()
    {
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            selectedDirection =
                intersectionSpace.OptionOneDirection;

            //Debug.Log($"Selected {selectedDirection}");

            waitingForConfirmation = true;
        }

        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            selectedDirection =
                intersectionSpace.OptionTwoDirection;

            //Debug.Log($"Selected {selectedDirection}");

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
        
        //Debug.Log($"Selected Direction: {selectedDirection}");
    }
    
    // Checks whether the specified direction is available at the current intersection
    public bool IsDirectionAvailable(
        PathDirection direction)
    {
        if (intersectionSpace == null)
            return false;

        return direction ==
               intersectionSpace.OptionOneDirection
               ||
               direction ==
               intersectionSpace.OptionTwoDirection;
    }
    
    
    // Applies the selected route and resumes movement.
    public void ConfirmDirection()
    {
        //Debug.Log($"Confirmed {selectedDirection}");

        BoardSpace selectedSpace;

        if (selectedDirection ==
            intersectionSpace.OptionOneDirection)
        {
            selectedSpace =
                intersectionSpace.NextSpaces[0];
        }
        else
        {
            selectedSpace =
                intersectionSpace.NextSpaces[1];
        }

        currentSpace = intersectionSpace;

        movementState =
            MovementState.Moving;

        waitingForConfirmation = false;

        StartCoroutine(ContinueMovementFromIntersection(selectedSpace));
    }

    private IEnumerator ContinueMovementFromIntersection(BoardSpace selectedSpace)
    {
        yield return StartCoroutine(MoveToSpace(selectedSpace));

        currentSpace = selectedSpace;

        if (currentSpace.SpaceType != SpaceType.Transit && currentSpace.SpaceType != SpaceType.Intersection)
        {
            remainingSpaces--;
        }
        
        movementState = MovementState.Moving;
        StartCoroutine(ContinueMovement());
    }
}
