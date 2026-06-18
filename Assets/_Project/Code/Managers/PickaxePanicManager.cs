using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickaxePanicManager : MonoBehaviour
{
    [Header("Game State")]
    [SerializeField] private PickaxePanicStates currentState;

    [Header("Timers")]
    [SerializeField] private float setupDuration = 5f;
    [SerializeField] private float playDuration = 10f;
    [SerializeField] private float resultsDuration = 5f;

    private float stateTimer;

    [Header("Scores")]
    [SerializeField] private int player1Score;
    [SerializeField] private int player2Score;
    [SerializeField] private int player3Score;
    [SerializeField] private int player4Score;

    private int winningPlayer;

    private void Start()
    {
        GameManager.Instance.ChangeGameState(GameState.Minigame);
        
        EnterSetupState();
    }
    
    private void Update()
    {
        switch (currentState)
        {
            case PickaxePanicStates.Setup:
                UpdateCountdown();
                break;
            
            case PickaxePanicStates.Playing:
                UpdateGameplay();
                break;
            
            case PickaxePanicStates.Results:
                UpdateResults ();
                break;
        }
    }

    private void EnterSetupState()
    {
        currentState = PickaxePanicStates.Setup;
        stateTimer = setupDuration;
        
        Debug.Log("Pickaxe Panic Setup Started");
    }

    private void UpdateCountdown()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0f) 
            EnterGameplayState();
    }

    private void EnterGameplayState()
    {
        currentState = PickaxePanicStates.Playing;
        stateTimer = playDuration;
        
        player1Score = 0;
        player2Score = 0;
        player3Score = 0;
        player4Score = 0;
        
        Debug.Log("Pickaxe Panic Gameplay Started");
    }

    private void UpdateGameplay()
    {
        stateTimer -= Time.deltaTime;
        
        if (InputManager.Instance.PlayerRollPressed(1))
        {
            player1Score++;
            //Debug.Log("Player 1 Score: " + player1Score);
        }
        if (InputManager.Instance.PlayerRollPressed(2))
        {
            player2Score++;
            //Debug.Log("Player 2 Score: " + player2Score);
        }
        if (InputManager.Instance.PlayerRollPressed(3))
        {
            player3Score++;
            //Debug.Log("Player 3 Score: " + player3Score);
        }
        if (InputManager.Instance.PlayerRollPressed(4))
        {
            player4Score++;
            //Debug.Log("Player 4 Score: " + player4Score);
        }

        if (stateTimer <= 0f)
        {
            DetermineWinner();
            EnterResultsState();
        }
    }

    private void DetermineWinner()
    {
        
        
        int winningPlayerIndex = 0;
        int highestScore = player1Score;

        if (player2Score > highestScore)
        {
            highestScore = player2Score;
            winningPlayerIndex = 1;
        }

        if (player3Score > highestScore)
        {
            highestScore = player3Score;
            winningPlayerIndex = 2;
        }

        if (player4Score > highestScore)
        {
            highestScore = player4Score;
            winningPlayerIndex = 3;
        }
        
        
        Debug.Log($"Player {winningPlayerIndex + 1} Wins!");
        Debug.Log($"P1: {player1Score}");
        Debug.Log($"P2: {player2Score}");
        Debug.Log($"P3: {player3Score}");
        Debug.Log($"P4: {player4Score}");
        
        PlayerData winnerData = GameManager.Instance.PlayerDataList[winningPlayerIndex];
        GameManager.Instance.MinigameWinner = winnerData;
    }

    private void EnterResultsState()
    {
        currentState = PickaxePanicStates.Results;
        GameEvents.OnGameStateRequested?.Invoke(GameState.Results);
        //Debug.Log("Showing Results");
        stateTimer = resultsDuration;

        SceneManager.LoadScene("MG_Results");
    }

    private void UpdateResults()
    {
        // FUTURE:
        // Move Camera to Winner
        // Play celebration animation
        // load result screen
    }
}
