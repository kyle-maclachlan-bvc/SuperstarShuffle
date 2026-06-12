using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance;
    
    [SerializeField] private List<MinigameData> fourPlayerMinigames;
    [SerializeField] private List<MinigameData> twoVsTwoMinigames;
    [SerializeField] private List<MinigameData> oneVsThreeMinigames;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    private MinigameTypes DetermineMinigameType()
    {
        // This will determine which minigame is played, by determining player colors
        return MinigameTypes.FourPlayer;
    }

    public MinigameData SelectMinigame()
    {
        MinigameTypes type = DetermineMinigameType();

        switch (type)
        {
            case MinigameTypes.FourPlayer:
                return GetRandomMinigame(fourPlayerMinigames);
            case MinigameTypes.TwoVsTwo:
                return GetRandomMinigame(twoVsTwoMinigames);
            case MinigameTypes.OneVsThree:
                return GetRandomMinigame(oneVsThreeMinigames);
        }

        return null;
        }
    
    private MinigameData GetRandomMinigame(List<MinigameData> list)
    {
        if (list.Count == 0)
            return null;
        return list[Random.Range(0, list.Count)];
    }
    }


