using UnityEngine;
using System.Collections.Generic;


public class BoardSpace : MonoBehaviour
{
    [SerializeField] private int spaceIndex; // Unique identifier for this board space; primarily for debugging and editor organization
    [SerializeField] private SpaceType spaceType; // Determines the gameplay behavior of this space
    [SerializeField] private List<BoardSpace> nextSpaces = new(); // List of board spaces that may be reached from this space.
    // Standard spaces contain one connection, Intersections contain multiple.
    [SerializeField] private MinigameTeams teamColor = MinigameTeams.None;
    [SerializeField] private PropertyOwner owner = PropertyOwner.None;

    [SerializeField] private PathDirection optionOneDirection; // Direction associated with the first available route at an intersection
    [SerializeField] private PathDirection optionTwoDirection; // Direction associated with the second available route at an intersection
    
    public PathDirection OptionOneDirection => optionOneDirection; // Get the first route direction available at this intersection
    public PathDirection OptionTwoDirection => optionTwoDirection; // Get the second route direction available at this intersection
    
    public int SpaceIndex => spaceIndex; // Gets the unique identifier for this space
    public SpaceType SpaceType => spaceType; // Gets the gameplay type for this board space
    public MinigameTeams TeamColor => teamColor;
    public List<BoardSpace> NextSpaces => nextSpaces; // Gets all board spaces directly connected to this space.

    public PropertyOwner Owner
    {
        get => owner;
        set => owner = value;
    }
        
    public bool IsPurchaseable =>
        spaceType != SpaceType.Happening &&
        spaceType != SpaceType.Transit &&
        spaceType != SpaceType.Intersection &&
        spaceType != SpaceType.Start;
    
    // Draws a visual representation of the board space inside the scene view only.
    // This gismo is used for board construction and debugging purposed and is not visible in-game.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.goldenRod;
        Gizmos.DrawSphere(transform.position, 0.25f);
    }
}
