using System.Collections;
using UnityEngine;

public class StartingCoinsManager : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.OnStartingCoinsPresentationRequested += GiveCoins;
    }

    private void OnDisable()
    {
        GameEvents.OnStartingCoinsPresentationRequested -= GiveCoins;
    }

    private void GiveCoins()
    {
        StartCoroutine(GiveCoinsRoutine());
    }

    private IEnumerator GiveCoinsRoutine()
    {
        foreach (Player player in GameManager.Instance.Players)
        {
            player.Currency.RemoveCoins(player.Currency.Coins);
        }

        HUDManager hud = FindFirstObjectByType<HUDManager>();

        for (int i = 0; i < 10; i++)
        {
            foreach (Player player in GameManager.Instance.Players)
            {
                player.Currency.AddCoins(1);
            }

            if (hud != null)
                hud.RefreshHUD();

            yield return new WaitForSeconds(0.12f);
        }
        
        GameEvents.OnStartingCoinsPresentationFinished?.Invoke();
    }
}
