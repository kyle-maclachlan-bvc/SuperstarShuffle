using UnityEngine;

/// <summary>
/// defines the current movement phase of the player
///
/// Responsibilities:
/// - Track whether a player is moving
/// - pauses movement at intersections
/// indicates when movement is complete
/// - Support movement state transitions during board traversal.
///
/// PlayerMovement uses these states to controle movement flow and determine when player input is required.
/// </summary>

public enum MovementState
{
    Idle,
    Moving,
    WaitingForDirection,
    WaitingForPropertyDecision
}

// Idle; the player is currently not moving
// This state is used when:
    // waiting for a die roll,
    // movement has completed
    // board events are resolving

// Moving; the player is actively traversing the board
// During this state:
    // remaining movement is consumed
    // Board spaces are traversed
    // Intersections are checked

// Waiting For Direction; The player has reached an intersection and must choose a route before movement continues
// During this state:
    // Movement is paused
    // Route selection input is accepted
    // remaining movement is preserved.