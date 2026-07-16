using System;

public static class GameEvents
{
    // Game Flow
    public static Action<GameFlowState> OnGameStateRequested;
    public static Action<GameFlowState> OnGameStateChanged;
    public static Action<string> OnSceneLoadRequested;
    public static Action OnBoardReady;
    
    // Game Setup
    public static Action OnGameSetupStarted;
    public static Action OnTurnOrderPresentationRequested;
    public static Action OnTurnOrderPresentationFinished;
    public static Action<Player> OnTurnOrderConfirmed;
    public static Action OnStartingCoinsPresentationRequested;
    public static Action OnStartingCoinsPresentationFinished;
    
    // Turn flow
    public static Action<Player> OnTurnStarted;
    public static Action<Player> OnTurnEnded;
    public static Action OnRoundCompleted;
    
    // Movement
    public static Action<Player, int> OnDiceRolled;
    public static Action<Player> OnMovementStarted;
    public static Action<Player> OnMovementFinished;
    public static Action<Player, int> OnMovementStepCompleted;
    
    // board flow
    public static Action<Player, BoardSpace> OnSpaceLanded;
    public static Action<Player, BoardSpace> OnSpaceResolutionStarted;
    public static Action<Player, BoardSpace> OnSpaceResolutionFinished;
    
    // Property
    public static Action<Player, BoardSpace> OnPropertyPurchaseRequested;
    public static Action<Player, BoardSpace> OnPropertyPurchased;
    public static Action<Player, BoardSpace> OnRentPaid;
    
    // Special Spaces
    public static Action<Player, BoardSpace> OnHappeningTriggered;
    
    // Minigames
    public static Action OnMinigameTutorialStarted;
    public static Action<PlayerData> OnMinigameWinnerDeclared;
    public static Action OnMinigameStarted;
    public static Action OnMinigameEnded;
    
    // Results
    public static Action OnResultsStarted;
    
    //UI Events
    public static Action<string, string[]> OnDialogueRequested;
    public static Action OnDialogueFinished;
    
    
}
