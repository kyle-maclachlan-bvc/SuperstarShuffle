using UnityEngine;
using System.Collections.Generic;
using Unity.Properties;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance; // Global reference to the active BoardManager. Singleton.
    
    [SerializeField] private List<BoardSpace> boardSpaces; // Collection of all board spaces on the board; Spaces are assigned through the Inspector

    // Initialize the singleton reference when the scene loads
    private void Awake()
    {
        Instance = this;
    }
    
    public BoardSpace GetSpace(int index)
    {
        foreach (BoardSpace space in boardSpaces)
        {
            if (space.SpaceIndex == index)
                return space;
        }
        
        return null;
    }

    public void RestoreOwnedProperties()
    {
        foreach (BoardSpace space in boardSpaces)
        {
            space.Owner = PropertyOwner.None;
        }
        
        foreach (Player player in GameManager.Instance.Players)
        {
            foreach (int spaceIndex in player.Data.OwnedSpaceIndices)
            {
                BoardSpace space = GetSpace(spaceIndex);

                if (space != null)
                {
                    space.Owner = player.PropertyOwner;
                }
            }
        }
    }
    
    public int SpaceCount => boardSpaces.Count;
}
