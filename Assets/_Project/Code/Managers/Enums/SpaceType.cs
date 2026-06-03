using UnityEngine;

/// <summary>
/// Defines the gameplay behavior of a board space.
///
/// Responsibilities:
/// - Categorize board spaces.
/// - Determine what occurs when a player lands on a space.
/// - Support board events and movement logic
/// - Provide a consistent way to identify space behavior
///
/// BoardSpace uses this enum to determine how players interact with each location on the board.
/// </summary>

public enum SpaceType
{
    Start,
    Blue,
    Red,
    Happening,
    Intersection,
    Transit,
    Star
}

// Start; the starting location of the board
// All players begin the game on this space. Players may pass by this space.

// Blue; a beneficial board space
// Planned behaviors:
    // Award coins to players
    // Represent the most common space type

// Red; a penalty board space
// Planned behaviors:
    // Remove coins from player
    // Create risk during board traversal

// Happening; a special event space
// Planned behaviors:
    // Trigger unique board events
    // Warp players
    // Activate board-specific mechanics

// Intersection; A branching path space
// Players must choose a route before movement can continue.
// This space type pauses movement and transitions the player into the Waiting for Direction movement state.

// Transit; a blank node used for navigation between spaces
// Players will automatically connect to these nodes between spaces so that they can move in more accurate directions.

// Star; the Star Space where players can buy stars.
// This is a blank node so players don't use up a space to walk on this space.
// Once the star is bought, it will move locations.