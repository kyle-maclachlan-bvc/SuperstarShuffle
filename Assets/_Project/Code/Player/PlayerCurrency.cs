using System;
using UnityEngine;

public class PlayerCurrency : MonoBehaviour
{
    private Player player;
    
    public int Coins => player.Data.Coins;

    private void Awake()
    {
        player = GetComponent<Player>();
    }
    
    public void AddCoins(int amount)
    {
        player.Data.Coins += amount;
    }

    /*public void RemoveCoins(int amount)
    {
        coins -= amount;
        Debug.Log($"{gameObject.name} now has {coins} coins");
    }*/

    public int RemoveCoins(int amount)
    {
        int coinsRemoved = Mathf.Min(player.Data.Coins, amount);

        player.Data.Coins -= coinsRemoved;

        return coinsRemoved;
    }


}
