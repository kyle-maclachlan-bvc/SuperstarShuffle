using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class ResultsManager : MonoBehaviour
{
    [FormerlySerializedAs("winnerCoinReward")]
    [Header("Results Settings")]
    [SerializeField] private int firstPlaceReward = 10;
    [SerializeField] private int secondPlaceReward = 5;
    [SerializeField] private int thirdPlaceReward = 3;
    [SerializeField] private int fourthPlaceReward = 0;

    [SerializeField] private ResultsUIManager[] rows;
    
    [SerializeField] private float minimumDisplayTime = 3f;
    
    
    private PlayerData winner;
    private float timer;
    private bool coinsAwarded;

    private class CoinRewardData
    {
        public PlayerData Player;
        public ResultsUIManager Row;
        public int RewardRemaining;
    }

    private void Start()
    {
       
        GameManager.Instance.ChangeGameState(GameState.Results);
        
        timer = minimumDisplayTime;
        
        winner = GameManager.Instance.MinigameWinner;
        
        LoadResults();
        PopulateResults();
        StartCoroutine(AnimateCoinReward());    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer > 0f)
            return;

        if (InputManager.Instance.PlayerRollPressed(1) || InputManager.Instance.PlayerRollPressed(2) ||
            InputManager.Instance.PlayerRollPressed(3) || InputManager.Instance.PlayerRollPressed(4))
            ReturnToBoard();
    }
    
    private void LoadResults()
    {
        if (winner == null)
        {
            Debug.LogError("No Minigame Winner Found");
            return;
        }
        
        Debug.Log($"{winner.PlayerName} Wins!");
    }

    private void PopulateResults()
    {
        int rowIndex = 0;

        foreach (MinigamePlacement placement in GameManager.Instance.MinigamePlacements)
        {
            foreach (PlayerData player in placement.Players)
            {
                rows[rowIndex].Initialize(player, placement.Place);
                rowIndex++;
            }
        }
    }

    private int GetRewardForPlace(int place)
    {
        switch (place)
        {
            case 1: return firstPlaceReward;
            case 2: return secondPlaceReward;
            case 3: return thirdPlaceReward;
            case 4: return fourthPlaceReward;
            default: return 0;
        }
    }

    private IEnumerator AnimateCoinReward()
    {
        yield return new WaitForSeconds(2f);
        
        List<CoinRewardData> rewards = new();
        
        int rowIndex = 0;

        foreach (MinigamePlacement placement in GameManager.Instance.MinigamePlacements)
        {
            int reward = GetRewardForPlace(placement.Place);

            foreach (PlayerData player in placement.Players)
            {
                rewards.Add(new CoinRewardData
                {
                    Player = player,
                    Row = rows[rowIndex],
                    RewardRemaining = reward
                });

                rowIndex++;
            }
        }

        bool animating = true;

        while (animating)
        {
            animating = false;
            foreach (CoinRewardData reward in rewards)
            {
                if (reward.RewardRemaining <= 0)
                    continue;

                reward.Player.Coins++;
                reward.Row.SetCoinText(reward.Player.Coins);
                
                reward.RewardRemaining--;
                animating = true;
            }

            if (animating)
                yield return new WaitForSeconds(0.08f);
        }
    }
    
    /*private void AwardCoins()
    {
        if (coinsAwarded)
            return;

        foreach (MinigamePlacement placement in GameManager.Instance.MinigamePlacements)
        {
            int reward = 0;

            switch (placement.Place)
            {
                case 1:
                    reward = firstPlaceReward;
                    break;

                case 2:
                    reward = secondPlaceReward;
                    break;

                case 3:
                    reward = thirdPlaceReward;
                    break;

                case 4:
                    reward = fourthPlaceReward;
                    break;
            }

            foreach (PlayerData player in placement.Players)
            {
                player.Coins += reward;

                Debug.Log($"{player.PlayerName} received {reward} coins.");
            }
        }

        coinsAwarded = true;
    }*/

    private void ReturnToBoard()
    {
        Debug.Log($"P1 Coins Before Reload: {GameManager.Instance.PlayerDataList[0].Coins}");
        Debug.Log($"P2 Coins Before Reload: {GameManager.Instance.PlayerDataList[1].Coins}");
        Debug.Log($"P3 Coins Before Reload: {GameManager.Instance.PlayerDataList[2].Coins}");
        Debug.Log($"P4 Coins Before Reload: {GameManager.Instance.PlayerDataList[3].Coins}");
        
        GameManager.Instance.ReturnFromMinigame = true;
        
        SceneManager.LoadScene("GlitterdeepMines");
    }
}
