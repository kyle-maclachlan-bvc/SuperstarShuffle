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

    private PlayerInput playerInput;
    private InputAction rollAction;
    private InputAction confirmAction;
    private InputAction cancelAction;
    private InputAction routeSelectAction;
    
    public bool ControlsEnabled { get; private set; }
    // Determines whether this player is currently allows to provide input; controlled by TurnManager.
    
    public int PlayerID => playerID;
    // Public read-only access to the playerID.

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        
        rollAction = playerInput.actions["Roll"];
        confirmAction = playerInput.actions["Confirm"];
        cancelAction = playerInput.actions["Cancel"];
        routeSelectAction = playerInput.actions["RouteSelect"];
    }
    
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
        
        Gamepad gamepad = InputManager.Instance.GetGamepad(playerID);
        bool controllerPressed = gamepad != null && gamepad.buttonSouth.wasPressedThisFrame;

        switch (playerID)
        {
            case 1:
                return Keyboard.current.qKey.wasPressedThisFrame || controllerPressed;
            
            case 2:
                return Keyboard.current.wKey.wasPressedThisFrame || controllerPressed;
            
            case 3:
                return Keyboard.current.eKey.wasPressedThisFrame || controllerPressed;
            
            case 4:
                return Keyboard.current.rKey.wasPressedThisFrame || controllerPressed;
        }

        return false;
    }

    public Vector2 RouteSelection()
    {
        if (!ControlsEnabled)
            return Vector2.zero;
        
        Gamepad gamepad = InputManager.Instance.GetGamepad(playerID);
        Vector2 input = Vector2.zero;
        
        // Keyboard Debugging
        // Keyboard Debug
        if (Keyboard.current.rightArrowKey.isPressed)
            input.x = 1;

        if (Keyboard.current.leftArrowKey.isPressed)
            input.x = -1;

        if (Keyboard.current.upArrowKey.isPressed)
            input.y = 1;

        if (Keyboard.current.downArrowKey.isPressed)
            input.y = -1;
        
        // Controllers
        if (gamepad != null)
        {
            Vector2 stickInput = gamepad.leftStick.ReadValue();

            if (stickInput.magnitude > 0.5f)
                input = stickInput;

            if (gamepad.dpad.right.isPressed)
                input = Vector2.right;

            if (gamepad.dpad.left.isPressed)
                input = Vector2.left;

            if (gamepad.dpad.up.isPressed)
                input = Vector2.up;

            if (gamepad.dpad.down.isPressed)
                input = Vector2.down;
        }

        return input;
    }

    public bool ConfirmPressed()
    {
        if (!ControlsEnabled)
            return false;
        
        Gamepad gamepad = InputManager.Instance.GetGamepad(playerID);
        bool controllerPressed = gamepad != null && gamepad.buttonSouth.wasPressedThisFrame;

        switch (playerID)
        {
            case 1:
                return Keyboard.current.enterKey.wasPressedThisFrame || controllerPressed;

            case 2:
                return Keyboard.current.enterKey.wasPressedThisFrame || controllerPressed;

            case 3:
                return Keyboard.current.enterKey.wasPressedThisFrame || controllerPressed;

            case 4:
                return Keyboard.current.enterKey.wasPressedThisFrame || controllerPressed;
        }

        return false;
    }
    
    public bool CancelPressed()
    {
        if (!ControlsEnabled)
            return false;

        Gamepad gamepad = InputManager.Instance.GetGamepad(playerID);
        bool controllerPressed = gamepad != null && gamepad.buttonSouth.wasPressedThisFrame;
        
        switch (playerID)
        {
            case 1:
                return Keyboard.current.backspaceKey.wasPressedThisFrame || controllerPressed;
            case 2:
                return Keyboard.current.backspaceKey.wasPressedThisFrame || controllerPressed;
            case 3:
                return Keyboard.current.backspaceKey.wasPressedThisFrame || controllerPressed;
            case 4:
                return Keyboard.current.backspaceKey.wasPressedThisFrame || controllerPressed;
        }

        return false;
    }
}
