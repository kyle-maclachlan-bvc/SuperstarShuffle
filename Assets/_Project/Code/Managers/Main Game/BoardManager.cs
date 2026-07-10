using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Stores and provides access to all board spaces.
///
/// Responsibilities:
/// - Maintain a collection of board spaces.
/// - Provide access to Spaces by Index
/// - Validate board space requests
/// - Serve as the central board reference point.
///
/// BoardManager does NOT control player movement.
/// It simply provides access to board data used by other systems such as PlayerMovement.
/// </summary>

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance; // Global reference to the active BoardManager. Singleton.
    
    [SerializeField] private List<BoardSpace> boardSpaces; // Collection of all board spaces on the board; Spaces are assigned through the Inspector

    // Initialize the singleton reference when the scene loads
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Debug.Log($"Board Loaded - Turn {GameManager.Instance.CurrentTurn}");
    }

    /*private void OnEnable()
    {
        GameEvents.OnBoardReady += HandleBoardReady;
    }

    private void OnDisable()
    {
        GameEvents.OnBoardReady -= HandleBoardReady;
    }*/
    
    // Retrieves a board space using its index.
    // Returns null if the requested index is invalid.
    public BoardSpace GetSpace(int index)
    {
        if (index < 0 || index >= boardSpaces.Count)
        {
            Debug.LogWarning($"Space {index} does not exist.");
            return null;
        }
        
        return boardSpaces[index];
    }

    /*private void HandleBoardReady()
    {
        Debug.Log("Board Reconstruction Complete");

        GameManager.Instance.RebuildTurnOrder();
        GameManager.Instance.ChangeGameState(GameState.PlayerTurn);
    }*/

    // Returns the total number of board spaces currently stored by the BoardManager
    public int SpaceCount => boardSpaces.Count;
}
