using UnityEngine;

/// <summary>
/// Central access point for all player-related systems
///
/// Responsibilities:
/// - Store references to player components.
/// - Provide a single entry point for accessing movement, controls, currency, and turn state.
/// - Reduce repeated GetComponent calls throughout the project.
///
/// This script acts as a hub that connects the various systems attached to the player game object.
/// </summary>

public class Player : MonoBehaviour
{
    public PlayerController Controller { get; private set; }
    // handles player input and control logic
    
    public PlayerMovement Movement { get;  private set; }
    // Handles player movement along the board
    
    public PlayerCurrency Currency { get;  private set; }
    // Stores and manages the player's coins and stars
    
    public PlayerTurnState TurnState { get;  private set; }
    // Tracks the player's current turn state.

    [SerializeField] private PropertyOwner propertyOwner;
    public PropertyOwner PropertyOwner => propertyOwner;

    private void Awake()
    {
        // Caches references to all player-related components attached to this game object.
        // Awake is sued so references are available before other systems begin interacting with the player.
        
        Controller = GetComponent<PlayerController>();
        Movement = GetComponent<PlayerMovement>();
        Currency = GetComponent<PlayerCurrency>();
        TurnState = GetComponent<PlayerTurnState>();
    }
}
