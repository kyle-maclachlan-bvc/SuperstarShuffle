using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultsManager : MonoBehaviour
{
    [Header("Results Settings")]
    [SerializeField] private int winnerCoinReward = 10;
    [SerializeField] private float minimumDisplayTime = 3f;
    
    private Player winner;
    private float timer;
    private bool coinsAwarded;

    private void Start()
    {
        //Debug.Log($"GameManager In Results = {GameManager.Instance.GetInstanceID()}");
        Debug.Log($"Winner In Results = {GameManager.Instance.MinigameWinner}");
        
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
        
        Debug.Log($"{winner.name} Wins!");
    }

    private void AwardCoins()
    {
        if (coinsAwarded)
            return;

        if (winner == null)
            return;
        
        winner.Currency.AddCoins(winnerCoinReward);
        
        Debug.Log($"{winner.name} recieved {winnerCoinReward} coins!");
        
        coinsAwarded = true;
    }

    private void ReturnToBoard()
    {
        Debug.Log("Returning to Board");
        SceneManager.LoadScene("Glitterdeep Mines");
    }
}
