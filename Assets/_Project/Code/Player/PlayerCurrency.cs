using System;
using UnityEngine;

public class PlayerCurrency : MonoBehaviour
{
    [SerializeField] private int coins = 0;

    public int Coins => coins;

    public void AddCoins(int amount)
    {
        coins += amount;
    }

    /*public void RemoveCoins(int amount)
    {
        coins -= amount;
        Debug.Log($"{gameObject.name} now has {coins} coins");
    }*/

    public int RemoveCoins(int amount)
    {
        int coinsRemoved = Mathf.Min(coins, amount);

        coins -= coinsRemoved;

        return coinsRemoved;
    }


}
