using UnityEngine;

/// <summary>
/// Defines a player's current role within the turn cycle.
///
/// Responsibilities:
/// - Track whether a player is active,
/// - Track wheter a palyer is waiting for their turn
/// - Track whether a player is currently resolving movement
/// - Identify players whos input is disabled.
///
/// Unlike MovementState, which tracks the mechanical details of board traversal,
/// Player state tracsk teh player's participation in the overall turn structure.
/// </summary>

public enum PlayerState
{
    Waiting,
    TakingTurn,
    Moving,
    Disabled
}

// Waiting; The player is inactive and waiting for their turn
// This is the default players who are not currently taking actions

// Taking Turn; The player is the active participant and may make gameplay decisions
// During this state, the player may:
    // roll the dice
    // Use items (eventually)
    // View the board
    // Review game information

// Moving; the player is currently resolving movement after rolling the dice
// During this state:
    // board traversal is occuring
    // Intersections may require route selection
    // MovementState controls the details of movement

// Disabled; the player's input is disabled
// This state may be used for:
    // AI-controlled players
