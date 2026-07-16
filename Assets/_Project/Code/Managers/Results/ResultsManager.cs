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

        var placements = GameManager.Instance.MinigamePlacements;

        if (placements.Count < 4)
            return;

        placements[0].Coins += firstPlaceReward;
        placements[1].Coins += secondPlaceReward;
        placements[2].Coins += thirdPlaceReward;
        placements[3].Coins += fourthPlaceReward;

        Debug.Log($"{placements[0].PlayerName} received {firstPlaceReward} coins.");
        Debug.Log($"{placements[1].PlayerName} received {secondPlaceReward} coins.");
        Debug.Log($"{placements[2].PlayerName} received {thirdPlaceReward} coins.");
        Debug.Log($"{placements[3].PlayerName} received {fourthPlaceReward} coins.");

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
