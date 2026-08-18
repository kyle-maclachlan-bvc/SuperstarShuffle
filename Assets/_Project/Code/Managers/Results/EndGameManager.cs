using UnityEngine;

public class EndGameManager : MonoBehaviour
{
    private Transform ceremonyCameraPoint;
    private int propertyValue = 5;
    private EndGameState currentEndGameState;
    public EndGameState CurrentEndGameState => currentEndGameState;

    private PlayerData winningPlayer;
    private int winningScore;
    
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
            
            highestCoinTotal = Mathf.Max(highestCoinTotal, player.Data.OwnedSpaceCount);
        }

        foreach (Player player in GameManager.Instance.Players)
        {
            //int finalCoinValue = player.Data.Coins + player.Data.PropertyBonusCoins;
            
            player.DiceUI.ShowValue(player.Data.OwnedSpaceCount);
            
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
                "And the winner is..."
            });
    }

    private void DetermineWinner()
    {
        winningPlayer = null;
        winningScore = 0;

        foreach (Player player in GameManager.Instance.Players)
        {
            int finalScore = player.Data.Coins + player.Data.PropertyBonusCoins;

            if (winningPlayer == null || finalScore > winningScore)
            {
                winningPlayer = player.Data;
                winningScore = finalScore;
            }
        }

        if (winningPlayer != null)
        {
            GameEvents.OnGameWinnerDeclared?.Invoke(winningPlayer);
        }
    }

    private void EndGame()
    {
        currentEndGameState = EndGameState.EndGameStarted;

        foreach (Player player in GameManager.Instance.Players)
        {
            player.DiceUI.Hide();
            player.DiceUI.StopPulse();
        }
    }
}