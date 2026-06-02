using UnityEngine;

/// <summary>
/// Stores and manages a player's current turn state.
///
/// Responsibilities:
/// - Track whether a player is active.
/// - Track whether a player is waiting.
/// - Track whether a player is moving
/// - Track whether a player is controlled by AI or otherwise unavailable for input.
///
/// This component acts as a central source of truth for player state information, and is updated primarily by the TurnManager.
/// </summary>

public class PlayerTurnState : MonoBehaviour
{
    [SerializeField] private PlayerState currentState; // The player's current gameplay state

    // Gets or sets the player's current state. Used by the TurnManager and other gameplay systems to determine what actions the player may perform
    public PlayerState CurrentState
    {
        get => currentState;
        set => currentState = value;
    }
}
