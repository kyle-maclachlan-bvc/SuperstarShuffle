using UnityEngine;

public class EndGameManager : MonoBehaviour
{
    private Transform ceremonyCameraPoint;
    private int propertyValue = 5;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    private void OnEnable()
    {
        GameEvents.OnEndGameStarted += BeginCeremony;
        GameEvents.OnEndGamePropertiesRevealStarted += RevealProperties;
    }

    private void OnDisable()
    {
        GameEvents.OnEndGameStarted -= BeginCeremony;
        GameEvents.OnEndGamePropertiesRevealStarted -= RevealProperties;
    }

    private void BeginCeremony()
    {
        RestorePlayers();

        RestoreCamera();
        
        CalculatePropertyBonuses();

        GameEvents.OnDialogueRequested?.Invoke(
            "Mine Foreman",
            new[]
            {
                "Well done, miners! The expedition has finally come to an end.",
                "Let's see how everyone performed!"
            });
        
        GameEvents.OnEndGamePropertiesRevealStarted?.Invoke();
    }

    private void RestorePlayers()
    {
        foreach (Player player in GameManager.Instance.Players)
        {
            player.Movement.MoveToStartingSpace();
        }
    }

    private void RestoreCamera()
    {
        if (ceremonyCameraPoint == null)
        {
            ceremonyCameraPoint = GameObject.Find("EndGameCameraPoint").transform;
        }

        Camera.main.transform.position =
            ceremonyCameraPoint.position;

        Camera.main.transform.rotation =
            ceremonyCameraPoint.rotation;
    }

    private void CalculatePropertyBonuses()
    {
        foreach (Player player in GameManager.Instance.Players)
        {
            player.Data.PropertyBonusCoins = player.Data.OwnedSpaceCount * propertyValue;
        }
    }

    private void RevealProperties()
    {
        int highestPropertyCount = 0;

        foreach (Player player in GameManager.Instance.Players)
        {
            highestPropertyCount = Mathf.Max(highestPropertyCount, player.Data.OwnedSpaceCount);
        }

        foreach (Player player in GameManager.Instance.Players)
        {
            player.DiceUI.ShowValue(player.Data.OwnedSpaceCount);
            
            if (player.Data.OwnedSpaceCount == highestPropertyCount)
                player.DiceUI.StartPulse();
            else
                player.DiceUI.StopPulse();
        }
    }
}