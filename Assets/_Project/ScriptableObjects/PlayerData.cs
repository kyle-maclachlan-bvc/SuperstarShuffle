using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Glitterdeep Mines/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Player Info")]
    public int PlayerID;
    public string PlayerName;

    [Header("Currency")]
    public int Coins;

    [Header("BoardProgress")]
    public int CurrentSpaceIndex;

    [Header("Properties")]
    public List<int> OwnedSpaceIndices = new();

    [Header("Statistics")]
    public int MinigameWins;

    [Header("Turn Order")]
    public int TurnOrderPosition;
}
