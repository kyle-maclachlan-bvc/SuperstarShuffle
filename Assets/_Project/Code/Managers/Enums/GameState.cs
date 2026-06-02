using UnityEngine;

/// <summary>
/// Defines the major phases of gameplay.
///
/// Responsibilities:
/// - Control overall game glow.
/// - Determines which systems are active
/// - Define transitions between menus, board gameplay, minigames, and results
///
/// GameManager uses these states to coordinate progression throughout a game.
/// </summary>

public enum GameState
{
    MainMenu,
    BoardSelection,
    GameSetup,
    PlayerTurn,
    MinigameTutorial,
    Minigame,
    Results,
    GameEnd
}

// Main Menu; the initial game state.
// Players may:
    // Start a new game
    // Access options
    // Exit the application

// Board Selection; Pre-game setup state
// Players choose:
    // Human or AI players
    // Character selections
    // Board selection
    // Number of turns

// Game Setup; Initialize board gameplay; operates on the board when turn order is being decided
// Responsibilities:
    // Spawn players
    // Initialize board systems
    // Determine player order
    // Prepares the first turn

// Player Turn; Main board gameplay state.
// Players:
    // Roll dice
    // move across the board
    // interact with spaces,
    // trigger board events

// Minigame Tutorial; Displays the instructions before a minigame
// Players can:
    // review objectives
    // learn controls
    // Read gameplay tips
// This state may be disabled through player settings

// Minigame; Active minigame gameplay
// Responsibilities:
    // Run minigame logic
    // Track player performance
    // Determine winners

// Results; Displays results after a minigame
// Responsibilities:
    // Award coins
    // Show rankings

// GameEnd, Final game state reached after all turns have been completed.
// Responsibilities:
    // Determine the final standings
    // Crown the winner