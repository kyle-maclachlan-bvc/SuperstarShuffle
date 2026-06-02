using UnityEngine;

/// <summary>
/// Automatically snaps hexagonal tiles to a flat-top hex grid while editing the board.
///
/// Responsibilities:
/// - Align tiles to the hex grid.
/// - Maintain consistent spacing.
/// - Prevent gaps between tiles.
/// - Assist with board construction
///
/// This script only operates in the Unity Editor and is intended as a level design utility
/// It is not used during gameplay.
/// </summary>

[ExecuteAlways]
public class HexSnap : MonoBehaviour
{
    public float radius = 2f; // Radius of the Hex Tile.

    private Vector3 lastPosition; // Stores the last recorded position of the tile. Used to detect when the tile has been moved in the Scene View.

    // Monitors tile movement while editing
    // If the tile position changes in the editor, the tile is automatically snapped to the nearest valid hex grid location.
    private void Update()
    {
        if (!Application.isPlaying)
        {
            if (transform.position != lastPosition)
            {
                SnapToGrid();
                lastPosition = transform.position;
            }
        }
    }

    // Align the tile to the nearest flat-top hexagonal grid position
    // Calculates the appropriate roow and column coordinates and moves the tile into alignment with neighboring hexes.
    private void SnapToGrid()
    {
        float xSpacing = radius * 1.5f; // Horizontal spacing between columns
        float zSpacing = Mathf.Sqrt(3f) * radius; // Vertical spacing between rows

        Vector3 pos = transform.position;

        int col = Mathf.RoundToInt(pos.x / xSpacing); // Determine which column is tile belongs to.

        float offset = (col % 2 == 0) ? 0f : zSpacing / 2f; // Odd columns are vertically offset by half a hex height.

        int row = Mathf.RoundToInt((pos.z - offset) / zSpacing); // Determine which row the tile belongs to.

        // Move the tile to its snapped position
        transform.position = new Vector3(
            col * xSpacing,
            pos.y,
            row * zSpacing + offset
        );
    }
}