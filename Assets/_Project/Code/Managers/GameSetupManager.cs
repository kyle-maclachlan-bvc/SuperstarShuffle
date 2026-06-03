using UnityEngine;

public class GameSetupManager : MonoBehaviour
{
    [SerializeField] private TurnManager turnManager;

    [Header("Player Rolls")]
    [SerializeField] private int PlayerOneRoll;
    [SerializeField] private int PlayerTwoRoll;
    [SerializeField] private int PlayerThreeRoll;
    [SerializeField] private int PlayerFourRoll;
    
    private bool playerOneRolled;
    private bool playerTwoRolled;
    private bool playerThreeRolled;
    private bool playerFourRolled;

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameState.GameSetup)
            return;
    }

    public void StartGameSetup()
    {
        Debug.Log("Game Setup Started");
            
            BeginBoardIntroduction();
    }

    private void BeginBoardIntroduction()
    {
        Debug.Log("Welcome to Glitterdeep Mines!");

        DetermineTurnOrder();
    }

    private void DetermineTurnOrder()
    {
        Debug.Log("DetermineTurnOrder");

        GiveStartingCoins();
    }

    private void GiveStartingCoins()
    {
        Debug.Log("Give Starting 10 Coins");

        BeginGameplay();
    }

    private void BeginGameplay()
    {
        Debug.Log("Begin Gameplay");
    }
}
