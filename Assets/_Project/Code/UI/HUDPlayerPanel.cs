using TMPro;
using UnityEngine;

public class HUDPlayerPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text propertyText;

    private Player player;

    public void SetPlayer(Player newPlayer)
    {
        player = newPlayer;
        Refresh();
    }

    public void Refresh()
    {
        if (player == null)
            return;

        playerNameText.text = player.Data.PlayerName;
        coinText.text = $"Coins: {player.Currency.Coins}";
        propertyText.text = $"Spaces: {player.Data.OwnedSpaceCount}";
    }
}
