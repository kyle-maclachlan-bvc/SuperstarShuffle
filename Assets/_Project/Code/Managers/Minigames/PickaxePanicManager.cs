using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [SerializeField] private float scoreTickTime = 0.1f;
    [SerializeField] private int scoreIncrement = 3;

    [Header("Winner Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float cameraMoveSpeed = 5f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 1.75f, 3.5f);
    private Player winnerPlayer;
    private List<Player> firstPlacePlayers = new();
    
    [Header("UI")]
    [SerializeField] private MinigameStartUI startUI;
    [SerializeField] private WinnerAnnouncementUI winnerUI;

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

    private void OnEnable()
    {
        InputManager.Instance.Controls.MinigamePickaxePanic.Pickaxe.performed += HandleGameplayInteraction;
    }

    private void OnDisable()
    {
        InputManager.Instance.Controls.MinigamePickaxePanic.Pickaxe.performed -= HandleGameplayInteraction;
    }
    
    private void HandleGameplayInteraction(InputAction.CallbackContext ctx) => UpdateGameplayInteraction(ctx);
    
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
        
        if (stateTimer <= 0f)
        {
            countdownText.text = "0";
                    
            startUI.ShowFinished();
                    
            DetermineWinner();
            EnterResultsState();
        }
    }

    private void UpdateGameplayInteraction(InputAction.CallbackContext context)
    {
        Player player1 = GameManager.Instance.Players[0];
        Player player2 = GameManager.Instance.Players[1];
        Player player3 = GameManager.Instance.Players[2];
        Player player4 = GameManager.Instance.Players[3];

        int playerIndex = context.control.name switch
        {
            "q" => 0,
            "w" => 1,
            "e" => 2,
            "r" => 3,
            _ => -1
        };

        if (playerIndex < 0)
            return;

        switch (playerIndex)
        {
            case 1:
                player1Score++;
                break;
            case 2:
                player2Score++;
                break;
            case 3:
                player3Score++;
                break;
            case 4:
                player4Score++;
                break;
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

        List<MinigamePlacement> placements = new();

        foreach (var result in results)
        {
            MinigamePlacement existing =
                placements.Find(p => p.Score == result.score);

            if (existing != null)
            {
                existing.Players.Add(result.player);
                continue;
            }

            int place = 1;

            foreach (MinigamePlacement p in placements)
                place += p.Players.Count;

            MinigamePlacement placement = new MinigamePlacement
            {
                Place = place,
                Score = result.score
            };

            placement.Players.Add(result.player);

            placements.Add(placement);
        }

        GameManager.Instance.SetMinigamePlacements(placements);

        GameEvents.OnMinigameWinnerDeclared?.Invoke(placements[0].Players[0]);

        firstPlacePlayers.Clear();

        foreach (Player player in FindObjectsByType<Player>(FindObjectsSortMode.None))
        {
            if (placements[0].Players.Contains(player.Data))
            {
                firstPlacePlayers.Add(player);
            }
        }

        Debug.Log("Final Placements");

        foreach (MinigamePlacement placement in placements)
        {
            string playerNames = "";

            foreach (PlayerData player in placement.Players)
            {
                if (playerNames.Length > 0)
                    playerNames += ", ";

                playerNames += player.PlayerName;
            }

            Debug.Log(
                $"{placement.Place}: {playerNames} ({placement.Score} Hits)");
        }
    }

    private void EnterResultsState()
    {
        currentState = PickaxePanicStates.Results;
        
        StartCoroutine(ResultSequence());
    }
    
    private IEnumerator AnimateScores()
    {
        int displayP1 = 0;
        int displayP2 = 0;
        int displayP3 = 0;
        int displayP4 = 0;

        player1HitText.gameObject.SetActive(true);
        player2HitText.gameObject.SetActive(true);
        player3HitText.gameObject.SetActive(true);
        player4HitText.gameObject.SetActive(true);

        bool finished = false;

        while (!finished)
        {
            finished = true;

            if (displayP1 < player1Score)
            {
                displayP1 = Mathf.Min(displayP1 + scoreIncrement, player1Score);
                finished = false;
            }

            if (displayP2 < player2Score)
            {
                displayP2 = Mathf.Min(displayP2 + scoreIncrement, player2Score);
                finished = false;
            }

            if (displayP3 < player3Score)
            {
                displayP3 = Mathf.Min(displayP3 + scoreIncrement, player3Score);
                finished = false;
            }

            if (displayP4 < player4Score)
            {
                displayP4 = Mathf.Min(displayP4 + scoreIncrement, player4Score);
                finished = false;
            }

            player1HitText.text = displayP1.ToString();
            player2HitText.text = displayP2.ToString();
            player3HitText.text = displayP3.ToString();
            player4HitText.text = displayP4.ToString();

            yield return new WaitForSeconds(scoreTickTime);
        }
    }
    
    private IEnumerator ResultSequence()
    {
        // Leave the 0 on screen for one second.
        yield return new WaitForSeconds(1f);

        // Remove the timer.
        countdownText.gameObject.SetActive(false);

        // Let the empty screen breathe.
        yield return new WaitForSeconds(1f);

        // Count everyone's score upward.
        yield return AnimateScores();

        // Let players admire the finished scores.
        yield return new WaitForSeconds(2f);

        player1HitText.gameObject.SetActive(false);
        player2HitText.gameObject.SetActive(false);
        player3HitText.gameObject.SetActive(false);
        player4HitText.gameObject.SetActive(false);

        FocusWinner();

        // Give the camera a moment to finish moving.
        yield return new WaitForSeconds(1f);

        // Show the winner banner.
        yield return winnerUI.Show(GameManager.Instance.MinigamePlacements[0]);

        SceneManager.LoadScene("MG_Results");
    }

    private void FocusWinner()
    {
        // If multiple players tied for first,
        // don't focus anyone.
        if (firstPlacePlayers.Count != 1)
        {
            Debug.Log("Tie for first place. No winner camera.");
            return;
        }

        winnerPlayer = firstPlacePlayers[0];

        StartCoroutine(FocusWinnerRoutine());
    }

    private IEnumerator FocusWinnerRoutine()
    {
        Vector3 startPos = mainCamera.transform.position;

        Vector3 targetPos =
            winnerPlayer.transform.position +
            winnerPlayer.transform.TransformDirection(cameraOffset);

        Quaternion targetRot =
            Quaternion.LookRotation(
                winnerPlayer.transform.position - targetPos,
                Vector3.up);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * cameraMoveSpeed;

            mainCamera.transform.position =
                Vector3.Lerp(startPos, targetPos, t);

            mainCamera.transform.rotation =
                Quaternion.Slerp(
                    mainCamera.transform.rotation,
                    targetRot,
                    t);

            yield return null;
        }
    }
}
