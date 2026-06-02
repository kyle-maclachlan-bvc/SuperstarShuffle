using UnityEngine;

/// <summary>
/// Legacy board position component.
///
/// Originally used to track player locations using numerical space indices.
///
/// The board system now uses BoardSpace references through PlayerMovement for graph-based navigation.
///
/// This script is retained TEMPORARILY for compatibility and may be removed in a future refactor.
/// </summary>

/// <summary>
/// Stores the players current position on the board as a space index.
///
/// Responsibilities:
/// - Track the player's board location
/// - provide access to the current board space index.
/// - support board progression systems that use numerical space references.


public class PlayerBoardPosition : MonoBehaviour
{
    [SerializeField] private int currentSpaceIndex = 0;
    // The index of the board space currently occupied by the player.

    public int CurrentSpaceIndex
    {
        get => currentSpaceIndex;
        set => currentSpaceIndex = value;
    }
}
