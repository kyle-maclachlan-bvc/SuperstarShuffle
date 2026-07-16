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
    
    [SerializeField] private float minimumDisplayTime = 3f;
    
    private PlayerData winner;
    private float timer;
    private bool coinsAwarded;

    private void Start()
    {
        //Debug.Log($"GameManager In Results = {GameManager.Instance.GetInstanceID()}");
        //Debug.Log($"Winner In Results = {GameManager.Instance.MinigameWinner}");
        
        GameManager.Instance.ChangeGameState(GameState.Results);
        
        timer = minimumDisplayTime;
        
        winner = GameManager.Instance.MinigameWinner;
        
        LoadResults();
        AwardCoins();
    }

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

    private void AwardCoins()
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
    }

    private void ReturnToBoard()
    {
        Debug.Log($"P1 Coins Before Reload: {GameManager.Instance.PlayerDataList[0].Coins}");
        Debug.Log($"P2 Coins Before Reload: {GameManager.Instance.PlayerDataList[1].Coins}");
        Debug.Log($"P3 Coins Before Reload: {GameManager.Instance.PlayerDataList[2].Coins}");
        Debug.Log($"P4 Coins Before Reload: {GameManager.Instance.PlayerDataList[3].Coins}");
        
        GameManager.Instance.ReturnFromMinigame = true;
        GameEvents.OnGameStateRequested?.Invoke(GameState.PlayerTurn);
        
        SceneManager.LoadScene("GlitterdeepMines");
    }
}
