using System.Collections.Generic;
using UnityEngine;

public class GameSetupManager : MonoBehaviour
{
    public void StartGameSetup()
    {
        Debug.Log("Welcome to Glitterdeep Mines!");
        //DetermineTurnOrder();1
        //GiveStartingCoins();
        //BeginGame();
    }
    
    private void DetermineTurnOrder()
    {
        List<Player> turnOrder = GameManager.Instance.TurnOrder;

        for (int i = 0; i < turnOrder.Count; i++)
        {
            int randomIndex = Random.Range(i, turnOrder.Count);

            Player temp = turnOrder[i];

            turnOrder[i] = turnOrder[randomIndex];
            
            turnOrder[randomIndex] = temp;
        }
        
        Debug.Log("Turn Order:");

        for (int i = 0; i < turnOrder.Count; i++)
        {
            Debug.Log($"{i + 1}: {turnOrder[i].name}");
        }
    }

    private void GiveStartingCoins()
    {
        
    }

    private void BeginGame()
    {
        
    }
}
