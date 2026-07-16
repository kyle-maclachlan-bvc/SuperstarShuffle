using System.Collections.Generic;

[System.Serializable]
public class MinigamePlacement
{
    public int Place;
    public int Score;

    public List<PlayerData> Players = new();
}