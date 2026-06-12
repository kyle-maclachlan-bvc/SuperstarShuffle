using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the overall flow of the game
///
/// Responsibilities:
/// - Track the current GameState
/// - Manage game progress
/// - Track completed turns
/// - Transition between gameplay phases
/// - Determines when the match ends
///
/// GameManager acts as the highest-level controller for the game and coordinates progression between board gameplay, minigames, and result screen
/// </summary>

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Global reference to the active GameManager. Singleton.

    [Header("Game State")]
    [SerializeField] private GameState currentGameState; // The current phase of the game.
    
    public GameState CurrentGameState => currentGameState; // Provides read-only access to the current game state

    [Header("Match Settings")]
    [SerializeField] private int currentTurn = 1; // Tracks the current turn of the match. Turn is completed when every player has taken their movement
    [SerializeField] private int maxTurns = 10; // The total number of turns that will be played before the game ends.
    // FUTURE, Turns can be adjusted to 15, 20, and 30.
    
    [Header("Game Managers")]
    [SerializeField] private GameSetupManager GameSetupManager;
    
    [Header("Turn Order")]
    [SerializeField] private List<Player> players;
    [SerializeField] private List<Player> turnOrder = new();
    public List<Player> Players => players;
    public List<Player> TurnOrder => turnOrder;

    [Header("Minigame Settings")]
    [SerializeField] private MinigameData currentMinigame;
    
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
    }

    // Starts the first gameplay phase when the scene loads
    private void Start()
    {
        ChangeGameState(GameState.GameSetup);
        GameSetupManager.StartGameSetup();
    }

    // Changes the current game state and notifies any subscribers.
    public void ChangeGameState(GameState newState)
    {
        currentGameState = newState;
        
        // Notify any systems listening for GameState changes.
        GameEvents.OnGameStateChange?.Invoke(newState);
        //Debug.Log(currentGameState.ToString());
    }

    // Called when all players have completed a round of board play.
    // Determines whether the match should continue, a minigame should begin, and the game has ended.
    public void RoundCompleted()
    {
        Debug.Log($"Turn {currentTurn} completed");

        if (currentTurn >= maxTurns)
        {
            Debug.Log($"Game Over");
            
            ChangeGameState(GameState.Results);

            return;
        }

        currentTurn++;

        //Later this will come from a Minigame Manager
        CurrentMinigame = MinigameManager.Instance.SelectMinigame();
        
        ChangeGameState(GameState.MinigameTutorial);

        SceneManager.LoadScene("MG_Tutorial");

    }
}
