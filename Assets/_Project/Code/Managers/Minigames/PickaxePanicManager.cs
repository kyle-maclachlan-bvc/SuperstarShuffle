using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickaxePanicManager : MonoBehaviour
{
    [Header("Game State")]
    [SerializeField] private PickaxePanicStates currentState;

    [Header("Timers")]
    [SerializeField] private float setupDuration = 2f;
    [SerializeField] private float playDuration = 10f;
    [SerializeField] private float resultsDuration = 5f;

    private float stateTimer;

    [Header("Scores")]
    [SerializeField] private int player1Score;
    [SerializeField] private int player2Score;
    [SerializeField] private int player3Score;
    [SerializeField] private int player4Score;

    [Header("UI")]
    [SerializeField] private MinigameStartUI startUI;

    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private TMP_Text player1HitText;
    [SerializeField] private TMP_Text player2HitText;
    [SerializeField] private TMP_Text player3HitText;
    [SerializeField] private TMP_Text player4HitText;
    
    private int winningPlayer;

    private void Start()
    {
        GameManager.Instance.ChangeGameState(GameState.Minigame);
        
        player1HitText.gameObject.SetActive(false);
        player2HitText.gameObject.SetActive(false);
        player3HitText.gameObject.SetActive(false);
        player4HitText.gameObject.SetActive(false);
        
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
        
        countdownText.text = "";
        startUI.ShowReady();
        
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
        
        countdownText.text = Mathf.CeilToInt(stateTimer).ToString();
        
        startUI.ShowGo();
        
        player1Score = 0;
        player2Score = 0;
        player3Score = 0;
        player4Score = 0;
        
        Debug.Log("Pickaxe Panic Gameplay Started");
    }

    private void UpdateGameplay()
    {
        stateTimer -= Time.deltaTime;
        
        countdownText.text = Mathf.Max(0, Mathf.CeilToInt(stateTimer)).ToString();
        
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
            countdownText.text = "0";
            
            DetermineWinner();
            EnterResultsState();
        }
    }

    private void DetermineWinner()
    {
        List<(PlayerData player, int score)> results = new()
        {
            (GameManager.Instance.PlayerDataList[0], player1Score),
            (GameManager.Instance.PlayerDataList[1], player2Score),
            (GameManager.Instance.PlayerDataList[2], player3Score),
            (GameManager.Instance.PlayerDataList[3], player4Score)
        };

        results.Sort((a, b) => b.score.CompareTo(a.score));

        List<PlayerData> placements = new();

        foreach (var result in results)
        {
            placements.Add(result.player);
        }

        GameManager.Instance.SetMinigamePlacements(placements);

        GameEvents.OnMinigameWinnerDeclared?.Invoke(placements[0]);

        Debug.Log("Final Placements");

        for (int i = 0; i < placements.Count; i++)
        {
            Debug.Log($"{i + 1}. {placements[i].PlayerName}");
        }
    }

    private void EnterResultsState()
    {
        currentState = PickaxePanicStates.Results;
        stateTimer = resultsDuration;

        player1HitText.gameObject.SetActive(true);
        player2HitText.gameObject.SetActive(true);
        player3HitText.gameObject.SetActive(true);
        player4HitText.gameObject.SetActive(true);

        player1HitText.text = $"{player1Score}";
        player2HitText.text = $"{player2Score}";
        player3HitText.text = $"{player3Score}";
        player4HitText.text = $"{player4Score}";

        Debug.Log("Showing Results");
    }

    private void UpdateResults()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer > 0f)
            return;

        GameEvents.OnGameStateRequested?.Invoke(GameState.Results);

        SceneManager.LoadScene("MG_Results");
    }
}
