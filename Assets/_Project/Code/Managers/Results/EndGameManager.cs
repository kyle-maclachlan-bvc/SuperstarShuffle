using UnityEngine;

public class EndGameManager : MonoBehaviour
{
    private Transform ceremonyCameraPoint;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    private void OnEnable()
    {
        GameEvents.OnEndGameStarted += BeginCeremony;
    }

    private void OnDisable()
    {
        GameEvents.OnEndGameStarted -= BeginCeremony;
    }

    private void BeginCeremony()
    {
        RestorePlayers();

        RestoreCamera();

        GameEvents.OnDialogueRequested?.Invoke(
            "Mine Foreman",
            new[]
            {
                "Well done, miners! The expedition has finally come to an end.",
                "Let's see how everyone performed!"
            });
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
}