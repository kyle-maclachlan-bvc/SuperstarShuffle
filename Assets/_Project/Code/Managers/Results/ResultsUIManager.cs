using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsUIManager : MonoBehaviour
{
    [SerializeField] private Image placementBadge;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text propertyText;

    [SerializeField] private Sprite firstPlaceBadge;
    [SerializeField] private Sprite secondPlaceBadge;
    [SerializeField] private Sprite thirdPlaceBadge;
    [SerializeField] private Sprite fourthPlaceBadge;

    public void SetCoinText(int amount)
    {
        coinText.text = amount.ToString();
    }
    
    public void Initialize(PlayerData player, int place)
    {
        playerNameText.text = player.PlayerName;
        coinText.text = player.Coins.ToString();
        propertyText.text = player.OwnedSpaceIndices.Count.ToString();

        switch (place)
        {
            case 1:
                placementBadge.sprite = firstPlaceBadge;
                break;
            case 2:
                placementBadge.sprite = secondPlaceBadge;
                break;
            case 3:
                placementBadge.sprite = thirdPlaceBadge;
                break;
            default:
                placementBadge.sprite = fourthPlaceBadge;
                break;
        }
    }
}
