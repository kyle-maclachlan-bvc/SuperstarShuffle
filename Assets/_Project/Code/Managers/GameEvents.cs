using System;

public static class GameEvents
{
    
    // Game Flow
    public static Action<GameState> OnGameStateRequested;
    public static Action<GameState> OnGameStateChange;
    public static Action OnBoardReady;
    
    // Turn flow
    public static Action<Player> OnTurnStarted;
    public static Action<Player> OnTurnEnded;
    public static Action OnRoundCompleted;
    
    // Movement
    public static Action<Player, int> OnDiceRolled;
    public static Action<Player> OnMovementStarted;
    public static Action<Player> OnMovementFinished;
    
    // board
    public static Action<Player, BoardSpace> OnSpaceLanded;
    
    // Minigames
    public static Action<PlayerData> OnMinigameWinnerDeclared;
    public static Action OnMinigameStarted;
    public static Action OnMinigameEnded;

    // Invokes whenever the game's current state changes.
    // Subscribers may use this event to update UI, enable systems, or react to game flow changes.

    //public static Action<Player> OnStarPurchased;
    // Planned event for notifying systems when a player purchases a star

    //public static Action<Player, int> OnCoinsChanged;
    // planned event for notifying when a player's coin total changes

    //public static Action OnMinigameStarted;
    // planned event for notifying systems when a minigame begins

    //public static Action OnMinigameEnded;
    // planned event for notifying systems when a minigame ends.
}
