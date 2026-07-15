using System;
using System.Collections;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private BoardSpace currentSpace; // The board node currently occupied by the player.
    [SerializeField] private MovementState movementState = MovementState.Idle; // Tracks the player's current Movement State.
    [SerializeField] private float moveSpeed = 5f; // movement speed for the Coroutine to move players between spaces.
    
    private int remainingSpaces; // Number of spaces remaining from the current dice roll.
    // This value decreases as the player moves across the board.
    
    private BoardSpace intersectionSpace; // Stores the intersection currently awaiting a route choice.

    private PlayerController playerController;
    private Player player;
    private PathDirection selectedDirection; // Route currently selected by player.
    private bool waitingForConfirmation; // Tracks whether a route has been selected and is waiting for player confirmation.
    //private BoardSpace pendingPropertySpace;
    //private bool waitingForPropertyDecision;
    
    
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

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
                player = GetComponent<Player>();
    }
    
    private void Start()
    {
        if (player != null)
        {
            Debug.Log(
                $"{player.Data.PlayerName} SAVED DATA BEFORE START = {player.Data.CurrentSpaceIndex}"
            );
        }
        
        BoardSpace[] allSpaces = FindObjectsByType<BoardSpace>(FindObjectsSortMode.None);

        foreach (BoardSpace space in allSpaces)
        {
            if (space.SpaceIndex == player.Data.CurrentSpaceIndex)
            {
                currentSpace = space;
                transform.position = space.transform.position;
                
                Debug.Log($"{player.Data.PlayerName} loaded onto space {space.SpaceIndex}");
                
                if (GameManager.Instance.ReturnFromMinigame)
                    GameManager.Instance.NotifyPlayerRestored();

                return;
            }
        }

        if (currentSpace != null)
        {
            transform.position = currentSpace.transform.position;
        }
    }

    // Begins a new movement sequence after a dice-block roll.
    public void StartMovingSpaces(int spaces)
    {
        movementState = MovementState.Moving;

        remainingSpaces = spaces;

        GameEvents.OnMovementStarted?.Invoke(player);

        StartCoroutine(ContinueMovement());
    }

    private void ConsumeMovement()
    {
        remainingSpaces--;
        GameEvents.OnMovementStepCompleted?.Invoke(player, remainingSpaces);
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
            
            if (player != null)
                player.Data.CurrentSpaceIndex = currentSpace.SpaceIndex;
            
            Debug.Log($"{player.Data.PlayerName} saved space {player.Data.CurrentSpaceIndex}");
            
            if (currentSpace.SpaceType != SpaceType.Transit && currentSpace.SpaceType != SpaceType.Intersection)
            {
                ConsumeMovement();
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

        GameEvents.OnSpaceLanded?.Invoke(player, currentSpace);
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
            ConsumeMovement();
        }
        
        movementState = MovementState.Moving;
        StartCoroutine(ContinueMovement());
    }
    
    public void FinishMovement()
    {
        PlayerMinigameTeams playerTeam = GetComponent<PlayerMinigameTeams>();

        if (playerTeam != null)
        {
            playerTeam.CurrentTeam = currentSpace.TeamColor;
            Debug.Log("Current Team: " + currentSpace.TeamColor);
        }

        movementState = MovementState.Idle;

        GameEvents.OnMovementFinished?.Invoke(player);
    }
}
