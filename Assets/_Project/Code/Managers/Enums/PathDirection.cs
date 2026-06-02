using UnityEngine;

/// <summary>
/// Defines the available movement directions that may be selected at board intersections
///
/// Responsibilities:
/// - Represent route choices at intersections
/// - Identify valid board traversal directions
/// - Support configurable path selection through BoardSpace data
///
/// BoardSpace uses these values to determine which routes are available when a player reaches an intersection
/// </summary>

public enum PathDirection
{
    Up,
    Down,
    Left,
    Right
}
