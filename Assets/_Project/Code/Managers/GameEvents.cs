using System;

/// <summary>
/// Centralized event system used for communication between gameplay systems.
///
/// Responsibilities:
/// - Boradcast major gameplay events.
/// - Reduce direct dependencies between systems.
/// - Support event-driven architecture
///
/// GameEvents allow systems to react to events without requiring direct references to another one.
/// GameManager may announce a GameState change, while UI, Audio, and other systems listen and respond independently.
/// </summary>

public static class GameEvents
{
    public static Action<GameState> OnGameStateChange;
    // Invokes whenever the game's current state changes.
    // Subscribers may use this event to update UI, enable systems, or react to game flow changes.

    //public static Action<int> OnTurnChange;
    // Planned event for notifying systems when the active player's turn changes.

    //public static Action<Player> OnStarPurchased;
    // Planned event for notifying systems when a player purchases a star

    //public static Action<Player, int> OnCoinsChanged;
    // planned event for notifying when a player's coin total changes

    //public static Action OnMinigameStarted;
    // planned event for notifying systems when a minigame begins
    
    //public static Action OnMinigameEnded;
    // planned event for notifying systems when a minigame ends.
}
