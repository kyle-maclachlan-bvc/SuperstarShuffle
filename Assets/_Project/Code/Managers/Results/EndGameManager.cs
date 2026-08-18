using System.Collections;
using UnityEngine;

public class EndGameManager : MonoBehaviour
{
    [SerializeField] private float cameraMoveSpeed = 5f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 1.75f, 3.5f);

    private Player winnerPlayer;
    private Coroutine winnerCameraRoutine;
    
    private Transform ceremonyCameraPoint;
    private int propertyValue = 5;
    private EndGameState currentEndGameState;
    public EndGameState CurrentEndGameState => currentEndGameState;

    private PlayerData winningPlayer;
    private int winningScore;
    private bool endGameActive;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    private void OnEnable()
    {
        GameEvents.OnEndGameStarted += BeginCeremony;
        GameEvents.OnDialogueFinished += HandleDialogueFinished;
    }

    private void OnDisable()
    {
        GameEvents.OnEndGameStarted -= BeginCeremony;
        GameEvents.OnDialogueFinished -= HandleDialogueFinished;
    }

    private void BeginCeremony()
    {
        endGameActive = true;
        currentEndGameState = EndGameState.EndGameStarted;
        
        RestorePlayers();
        RestoreCamera();
        
        CalculatePropertyBonuses();

        GameEvents.OnDialogueRequested?.Invoke(
            "Mine Foreman",
            new[]
            {
                "Well done, miners! The expedition has finally come to an end.",
                "Let's see how everyone performed!"
            });
    }

    private void RestorePlayers()
    {
        foreach (Player player in GameManager.Instance.Players)
        {
            player.Movement.MoveToStartingSpace();
        }
    }

    private void RestoreCamera()
    {
        if (ceremonyCameraPoint == null)
        {
            ceremonyCameraPoint = GameObject.Find("EndGameCameraPoint").transform;
        }

        Camera.main.transform.position =
            ceremonyCameraPoint.position;

        Camera.main.transform.rotation =
            ceremonyCameraPoint.rotation;
    }

    private void CalculatePropertyBonuses()
    {
        foreach (Player player in GameManager.Instance.Players)
        {
            player.Data.PropertyBonusCoins = player.Data.OwnedSpaceCount * propertyValue;
        }
    }

    private void HandleDialogueFinished()
    {
        if (!endGameActive) return;
        
        switch (currentEndGameState)
        {
            case EndGameState.EndGameStarted:
                StartPropertyReveal();
                break;
            case EndGameState.PropertyRevealed:
                StartCoinReveal();
                break;
            case EndGameState.CoinsRevealed:
                StartWinnerAnnouncement();
                break;
            case EndGameState.WinnerAnnounced:
                EndGame();
                break;
        }
    }

    private void StartPropertyReveal()
    {
        currentEndGameState = EndGameState.PropertyRevealed;
        RevealProperties();
        GameEvents.OnDialogueRequested?.Invoke(
            "Mine Foreman",
            new[]
            {
                "You all sure bought quite a few properties.",
                "Now let's look at your coin totals."
            });
    }

    private void RevealProperties()
    {
        int highestPropertyCount = 0;

        foreach (Player player in GameManager.Instance.Players)
        {
            highestPropertyCount = Mathf.Max(highestPropertyCount, player.Data.OwnedSpaceCount);
        }

        foreach (Player player in GameManager.Instance.Players)
        {
            player.DiceUI.ShowValue(player.Data.OwnedSpaceCount);
            
            if (player.Data.OwnedSpaceCount == highestPropertyCount)
                player.DiceUI.StartPulse();
            else
                player.DiceUI.StopPulse();
        }
    }

    private void StartCoinReveal()
    {
        currentEndGameState = EndGameState.CoinsRevealed;
        RevealCoins();
        GameEvents.OnDialogueRequested?.Invoke(
            "Mine Foreman",
            new[]
            {
                "You've all gathered so many coins in those minigames.",
                "It's time to reveal who won by having the most properties and coins.",
            });
    }

    private void RevealCoins()
    {
        int highestCoinTotal = 0;

        foreach (Player player in GameManager.Instance.Players)
        {
            //int finalCoinValue = player.Data.Coins + player.Data.PropertyBonusCoins;
            //highestCoinTotal = Mathf.Max(highestCoinTotal, finalCoinValue);
            
            highestCoinTotal = Mathf.Max(highestCoinTotal, player.Data.Coins);
        }

        foreach (Player player in GameManager.Instance.Players)
        {
            //int finalCoinValue = player.Data.Coins + player.Data.PropertyBonusCoins;
            
            player.DiceUI.ShowValue(player.Data.Coins);
            
            if (player.Data.Coins == highestCoinTotal)
                player.DiceUI.StartPulse();
            else
                player.DiceUI.StopPulse();
            
        }
    }

    private void StartWinnerAnnouncement()
    {
        currentEndGameState = EndGameState.WinnerAnnounced;
        DetermineWinner();
        GameEvents.OnDialogueRequested?.Invoke(
            "Mine Foreman",
            new[]
            {
                "And the winner is...",
                winningPlayer.PlayerName + "!"
            });
        FocusWinner();
    }

    private void DetermineWinner()
    {
        winningPlayer = null;
        winnerPlayer = null;
        winningScore = 0;

        foreach (Player player in GameManager.Instance.Players)
        {
            int finalScore =
                player.Data.Coins +
                player.Data.PropertyBonusCoins;

            if (winningPlayer == null || finalScore > winningScore)
            {
                winningPlayer = player.Data;
                winnerPlayer = player;
                winningScore = finalScore;
            }
        }

        if (winningPlayer != null)
        {
            GameEvents.OnGameWinnerDeclared?.Invoke(winningPlayer);
        }
    }

    private void FocusWinner()
    {
        if (winnerPlayer == null) return;
        if (winnerCameraRoutine != null)
            StopCoroutine(winnerCameraRoutine);

        winnerCameraRoutine = StartCoroutine(FocusWinnerRoutine());
    }

    private IEnumerator FocusWinnerRoutine()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera == null) yield break;
        
        Vector3 startPos = mainCamera.transform.position;

        Vector3 targetPos = winnerPlayer.transform.position + winnerPlayer.transform.TransformDirection(cameraOffset);
        Quaternion targetRot = Quaternion.LookRotation(winnerPlayer.transform.position - targetPos, Vector3.up);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * cameraMoveSpeed;

            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, targetRot, t);

            yield return null;
        }
        
        mainCamera.transform.position = targetPos;
        mainCamera.transform.rotation = targetRot;
        
        winnerCameraRoutine = null;
    }

    private void EndGame()
    {
        currentEndGameState = EndGameState.EndTheGame;

        foreach (Player player in GameManager.Instance.Players)
        {
            player.DiceUI.Hide();
            player.DiceUI.StopPulse();
        }
    }
}