using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player-specific input checks.
///
/// Responsibilities:
/// - Determine whether a player's controls are enabled.
/// - Detect board gameplay inputs.
/// - Restrict input to the active player.
/// - Provide a centralized interface for TurnManager and board systems to query player actions.
///
/// During development, keyboard inputs are used for debugging multiplayer functionality:
/// P1 = Q
/// P2 = W
/// P3 = E
/// P4 = R
///
/// Future versions may replace these bindings with Input Actions from the MarioPartyControls Asset.
/// </summary>

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int playerID;
    // Identifies which player this controller belongs to.
    // Used to determine which debug input key should be read.
    
    public bool ControlsEnabled { get; private set; }
    // Determines whether this player is currently allows to provide input; controlled by TurnManager.
    
    public int PlayerID => playerID;
    // Public read-only access to the playerID.

    public void EnableControls()
    {
        // Enables player input. Called when the player's turn begins.
        
        ControlsEnabled = true;
    }
    
    public void DisableControls()
    {
        // Disables player input. Called when the player's turn ends or when another player becomes active.
        
        ControlsEnabled = false;
    }

    public bool RollPressed()
    {
        // Checks whether this player has pressed their assigned dice roll button.
        // Only returns true if controls are enabled.
        
        // Ignore input when controls are disabled.
        if (!ControlsEnabled)
            return false;

        switch (playerID)
        {
            case 1:
                return Keyboard.current.qKey.wasPressedThisFrame;
            
            case 2:
                return Keyboard.current.wKey.wasPressedThisFrame;
            
            case 3:
                return Keyboard.current.eKey.wasPressedThisFrame;
            
            case 4:
                return Keyboard.current.rKey.wasPressedThisFrame;
        }

        return false;
    }
    
    public bool SelectRightPressed()
    {
        // Checks if the player selected the right route at board intersection
        
        if (!ControlsEnabled)
            return false;

        return Keyboard.current.rightArrowKey.wasPressedThisFrame;
    }

    public bool SelectDownPressed()
    {
        // Checks if the player selected the downward route at board intersection
        
        if (!ControlsEnabled)
            return false;

        return Keyboard.current.downArrowKey.wasPressedThisFrame;
    }
    
    public bool SelectLeftPressed()
    {
        // Checks if the player selected the left route at board intersection
        
        if (!ControlsEnabled)
            return false;

        return Keyboard.current.leftArrowKey.wasPressedThisFrame;
    }

    public bool SelectUpPressed()
    {
        // Checks if the player selected the upward route at board intersection
        
        if (!ControlsEnabled)
            return false;

        return Keyboard.current.upArrowKey.wasPressedThisFrame;
    }

    public bool ConfirmPressed()
    {
        // Checks if the player has confirmed their current route selection
        
        if (!ControlsEnabled)
            return false;

        return Keyboard.current.enterKey.wasPressedThisFrame;
    }
}
