using UnityEngine;

[System.Serializable]
public class BoardData
{
    public PropertyOwner[] SpaceOwners;

    public BoardData(int totalSpaces)
    {
        SpaceOwners = new PropertyOwner[totalSpaces];
        ResetBoard();
    }

    public void ResetBoard()
    {
        for (int i = 0; i < SpaceOwners.Length; i++)
        {
            SpaceOwners[i] = PropertyOwner.None;
        }
    }

    public PropertyOwner GetOwner(int spaceIndex)
    {
        return SpaceOwners[spaceIndex];
    }

    public void SetOwner(int spaceIndex, PropertyOwner owner)
    {
        SpaceOwners[spaceIndex] = owner;
    }
    
    
}
