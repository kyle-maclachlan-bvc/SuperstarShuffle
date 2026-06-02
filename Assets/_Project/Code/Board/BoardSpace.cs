using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a single node on the game board.
///
/// Responsibilities:
/// - Store board space information
/// - Maintain connections to other board spaces
/// - Define the type of space
/// - Store intersection rout options
/// - Provide board graph data used by PlayerMovement
///
/// Every traversable location on the board is represented by a BoardSpace, including:
/// Start, Blue, Red, Happening, Intersections
/// BoardSpaces form a graph structure by connecting to other board spaces through the NextSpace list.
/// </summary>

public class BoardSpace : MonoBehaviour
{
    [SerializeField] private int spaceIndex; // Unique identifier for this board space; primarily for debugging and editor organization
    [SerializeField] private SpaceType spaceType; // Determines the gameplay behavior of this space
    [SerializeField] private List<BoardSpace> nextSpaces = new(); // List of board spaces that may be reached from this space.
    // Standard spaces contain one connection, Intersections contain multiple.

    [SerializeField] private PathDirection optionOneDirection; // Direction associated with the first available route at an intersection
    [SerializeField] private PathDirection optionTwoDirection; // Direction associated with the second available route at an intersection
    
    public PathDirection OptionOneDirection => optionOneDirection; // Get the first route direction available at this intersection
    public PathDirection OptionTwoDirection => optionTwoDirection; // Get the second route direction available at this intersection
    
    public int SpaceIndex => spaceIndex; // Gets the unique identifier for this space
    public SpaceType SpaceType => spaceType; // Gets the gameplay type for this board space
    public List<BoardSpace> NextSpaces => nextSpaces; // Gets all board spaces directly connected to this space.
    
    // Draws a visual representation of the board space inside the scene view only.
    // This gismo is used for board construction and debugging purposed and is not visible in-game.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.goldenRod;
        Gizmos.DrawSphere(transform.position, 0.25f);
    }
}
