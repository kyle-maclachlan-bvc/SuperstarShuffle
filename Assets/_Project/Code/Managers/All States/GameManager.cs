using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Global reference to the active GameManager. Singleton.

    [Header("Game State")]
    //[SerializeField] private GameState currentGameState; // The current phase of the game.
    private StateMachine gameStateMachine;
    public StateMachine StateMachine => gameStateMachine;
    
    //public GameState CurrentGameState => currentGameState; // Provides read-only access to the current game state

    [Header("Match Settings")]
    [SerializeField] private int currentTurn = 1; // Tracks the current turn of the match. Turn is completed when every player has taken their movement
    [SerializeField] private int maxTurns = 10; // The total number of turns that will be played before the game ends.
    // FUTURE, Turns can be adjusted to 15, 20, and 30
    
    [SerializeField] private bool matchInitialized;
    
    public int CurrentTurn => currentTurn;
    public bool  MatchInitialized => matchInitialized;
    
    [Header("Game Managers")]
    [SerializeField] private GameSetupManager gameSetupManager;
    public GameSetupManager GameSetupManager => gameSetupManager;
    
    [Header("Turn Order")]
    [SerializeField] private List<Player> players;
    [SerializeField] private List<Player> turnOrder = new();
    public List<Player> Players => players;
    public List<Player> TurnOrder => turnOrder;

    [Header("Minigame Settings")] [SerializeField]
    private List<PlayerData> playerDataList = new();
    [SerializeField] private MinigameData currentMinigame;
    [SerializeField] private PlayerData minigameWinner;
    [SerializeField] private bool returnFromMinigame;
    
    [SerializeField] private int playersRestored;
    
    public List<PlayerData> PlayerDataList => playerDataList;

    public PlayerData MinigameWinner
    {
        get => minigameWinner;
        set => minigameWinner = value;
    }

    public bool ReturnFromMinigame
    {
        get => returnFromMinigame;
        set => returnFromMinigame = value;
    }

    public MinigameData CurrentMinigame
    {
        get => currentMinigame;
        set => currentMinigame = value;
    }

    // Initializes the GameManager Singleton, and ensures only one exists
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        gameStateMachine = new StateMachine();
    }

    // Starts the first gameplay phase when the scene loads
    private void Start()
    {
        if (GameSetupManager == null)
            return;

        if (matchInitialized)
            return;
        
        ResetPlayerData();
        
        matchInitialized = true;
        
        //ChangeGameState(GameState.GameSetup);
        gameStateMachine.ChangeState(new GameSetupState(this));
    }

    private void Update()
    {
        gameStateMachine.Update();
    }

    private void OnEnable()
    {
        GameEvents.OnBoardReady += HandleBoardReady;
        GameEvents.OnRoundCompleted += HandleRoundCompleted;
        GameEvents.OnGameStateRequested += HandleGameStateRequested;
        GameEvents.OnMinigameWinnerDeclared += HandleMinigameWinnerDeclared;
        GameEvents.OnSpaceLanded += HandleSpaceLanded;
    }

    private void OnDisable()
    {
        GameEvents.OnBoardReady -= HandleBoardReady;
        GameEvents.OnRoundCompleted -= HandleRoundCompleted;
        GameEvents.OnGameStateRequested -= HandleGameStateRequested;
        GameEvents.OnMinigameWinnerDeclared -= HandleMinigameWinnerDeclared;
        GameEvents.OnSpaceLanded -= HandleSpaceLanded;
    }

    // Changes the current game state and notifies any subscribers.
    /*public void ChangeGameState(GameState newState)
    {
        currentGameState = newState;
        
        Debug.Log($"Game State: {newState}");
        
        GameEvents.OnGameStateChanged?.Invoke(newState);
    }*/
    
    public void RebuildTurnOrder()
    {
        players.Clear();
        turnOrder.Clear();

        Player[] scenePlayers = FindObjectsByType<Player>(FindObjectsSortMode.None);

        List<Player> sortedPlayers = new List<Player>(scenePlayers);
        
        sortedPlayers.Sort((a,b) => a.Data.TurnOrderPosition.CompareTo(b.Data.TurnOrderPosition));

        foreach (Player player in sortedPlayers)
        {
            players.Add(player);
            turnOrder.Add(player);
            
            Debug.Log($"{player.Data.PlayerName} Restored at turn position {player.Data.TurnOrderPosition}");
        }
        
        Debug.Log($"Rebuilt Turn Order with {turnOrder.Count} players");
    }

    private void HandleGameStateRequested(GameFlowState newState)
    {
        gameStateMachine.ChangeState(newState);

        Debug.Log($"Game State -> {newState.GetType().Name}");

        GameEvents.OnGameStateChanged?.Invoke(newState);
    }

    private void HandleBoardReady()
    {
        Debug.Log("Board Reconstruction Complete");
        RebuildTurnOrder();
        gameStateMachine.ChangeState(new BoardGameplayState(this));
    }

    private void HandleSpaceLanded(Player player, BoardSpace space)
    {
        Debug.Log($"{player.Data.PlayerName} landed on {space.SpaceIndex}");
    }

    private void HandleRoundCompleted()
    {
        RoundCompleted();
    }
    
    // Called when all players have completed a round of board play.
    // Determines whether the match should continue, a minigame should begin, and the game has ended.
    public void RoundCompleted()
    {
        Debug.Log($"Turn {currentTurn} completed");

        if (currentTurn >= maxTurns)
        {
            Debug.Log($"Game Over");

            gameStateMachine.ChangeState(new ResultsState(this));

            return;
        }

        currentTurn++;

        //Later this will come from a Minigame Manager
        CurrentMinigame = MinigameManager.Instance.SelectMinigame();
        
        gameStateMachine.ChangeState(new MinigameTutorialState(this));

        GameEvents.OnSceneLoadRequested?.Invoke("MG_Tutorial");
    }

    private void HandleMinigameWinnerDeclared(PlayerData winner)
    {
        MinigameWinner = winner;
        Debug.Log($"Winner Stored: {winner.PlayerName}");
    }
    
    public void NotifyPlayerRestored()
    {
        playersRestored++;
        
        Debug.Log($"Players Restored: {playersRestored}/{TurnOrder.Count}");
        
        if (playersRestored >= TurnOrder.Count)
            {
                Debug.Log("All Players Stroed");
                playersRestored = 0;
                ReturnFromMinigame = false;
                GameEvents.OnBoardReady?.Invoke();
            }
    }
    
    // Short-Term positon fix:
    private void ResetPlayerData()
    {
        foreach (PlayerData data in playerDataList)
        {
            data.Coins = 0;
            data.CurrentSpaceIndex = -1;
            data.MinigameWins = 0;
            data.OwnedSpaceIndices.Clear();
        }
    }
}
