using NUnit.Framework.Api;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

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
    [SerializeField] private bool allowKeyboardDebug = true;
    
    private PlayerInput playerInput;
    //private InputAction rollAction;
    //private InputAction confirmAction;
    //private InputAction cancelAction;
    //private InputAction routeSelectAction;
    
    
    public bool ControlsEnabled { get; private set; }
    // Determines whether this player is currently allows to provide input; controlled by TurnManager.
    
    public int PlayerID => playerID;
    // Public read-only access to the playerID.

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        
        Debug.Log($"{name} PlayerInput Found = {playerInput != null}");
        
        //rollAction = playerInput.actions["Roll"];
        //confirmAction = playerInput.actions["Confirm"];
        //cancelAction = playerInput.actions["Cancel"];
        //routeSelectAction = playerInput.actions["RouteSelect"];
        
        //Debug.Log($"{name} Roll Action Hash = {rollAction.GetHashCode()}");
    }
    
    private void Start()
    {
        Debug.Log($"{name} Player Index = {playerInput.playerIndex}");
        Debug.Log($"{name} Device Count = {playerInput.devices.Count}");

        foreach (var device in playerInput.devices)
        {
            Debug.Log($"{name} Device = {device.displayName}");
        }
    }
    
    public void EnableControls()
    {
        // Enables player input. Called when the player's turn begins.
        
        ControlsEnabled = true;
        
        Debug.Log($"{gameObject.name} Controls Enabled");
    }
    
    public void DisableControls()
    {
        // Disables player input. Called when the player's turn ends or when another player becomes active.
        
        ControlsEnabled = false;
        
        Debug.Log($"{gameObject.name} Controls Disabled");
    }

    public bool RollPressed()
    {
        var roll = playerInput.currentActionMap["Roll"];

        Debug.Log(
            $"{name} Roll ActiveControl = " +
            $"{roll.activeControl?.device?.displayName}"
        );
        
        if (!ControlsEnabled)
            return false;

        if (playerInput.currentActionMap["Roll"].WasPressedThisFrame())
        {
            Debug.Log($"{name} detected controller roll");
            return true;
        }

        if (allowKeyboardDebug)
        {
            switch (playerID)
            {
                case 1:
                    if (Keyboard.current.qKey.wasPressedThisFrame)
                    {
                        Debug.Log($"{name} detected keyboard roll");
                        return true;
                    }
                    break;

                case 2:
                    if (Keyboard.current.wKey.wasPressedThisFrame)
                    {
                        Debug.Log($"{name} detected keyboard roll");
                        return true;
                    }
                    break;

                case 3:
                    if (Keyboard.current.eKey.wasPressedThisFrame)
                    {
                        Debug.Log($"{name} detected keyboard roll");
                        return true;
                    }
                    break;

                case 4:
                    if (Keyboard.current.rKey.wasPressedThisFrame)
                    {
                        Debug.Log($"{name} detected keyboard roll");
                        return true;
                    }
                    break;
            }
        }

        return false;
    }

    private Vector2 RouteInput()
    {
        return playerInput.currentActionMap["RouteSelect"].ReadValue<Vector2>();
        
        //return routeSelectAction.ReadValue<Vector2>();
        
        //return InputManager.Instance.Controls.BoardGame.RouteSelect.ReadValue<Vector2>();
    }
    
    public bool SelectRightPressed()
    {
        // Checks if the player selected the right route at board intersection
        
        if (!ControlsEnabled)
            return false;

        return RouteInput().x > 0.5f;
    }

    public bool SelectDownPressed()
    {
        // Checks if the player selected the downward route at board intersection
        
        if (!ControlsEnabled)
            return false;

        return RouteInput().y < -0.5f;
    }
    
    public bool SelectLeftPressed()
    {
        // Checks if the player selected the left route at board intersection
        
        if (!ControlsEnabled)
            return false;

        return RouteInput().x < -0.5f;
    }

    public bool SelectUpPressed()
    {
        // Checks if the player selected the upward route at board intersection
        
        if (!ControlsEnabled)
            return false;

        return RouteInput().y > 0.5f;
    }

    public bool ConfirmPressed()
    {
        // Checks if the player has confirmed their current route selection
        
        if (!ControlsEnabled)
            return false;
        
        // Controller assigned to this player
        if (playerInput.currentActionMap["Confirm"].WasPressedThisFrame())
            return true;
        
        //if (confirmAction.WasPressedThisFrame())
            //return true;
        
        // Keyboard debug controls
        if (allowKeyboardDebug)
            return Keyboard.current.enterKey.wasPressedThisFrame;

        return false; ;
    }

    private bool CancelPressed()
    {
        if (!ControlsEnabled)
            return false;
        
        // Controller assigned to this player
        if (playerInput.currentActionMap["Cancel"].WasPressedThisFrame())
            return true;
        
        //if (cancelAction.WasPressedThisFrame())
            //return true;
            
        // Keyboard debug controls
        if (allowKeyboardDebug)
            return Keyboard.current.backspaceKey.wasPressedThisFrame;

        return false;
    }

    public void PairDevice(InputDevice device)
    {
        Debug.Log($"{name} playerInput = {playerInput}");

        if (playerInput == null)
        {
            Debug.LogError($"{name} PlayerInput is NULL");
            return;
        }

        Debug.Log($"{name} user valid = {playerInput.user.valid}");

        playerInput.user.UnpairDevices();

        InputUser.PerformPairingWithDevice(device, playerInput.user);

        Debug.Log($"{name} paired with {device.displayName}");
    }
    
    public void LogDeviceInfo()
    {
        Debug.Log($"----- {name} Device Report -----");

        Debug.Log($"{name} User ID = {playerInput.user.id}");
        Debug.Log($"{name} Device Count = {playerInput.devices.Count}");

        foreach (var device in playerInput.devices)
        {
            Debug.Log($"{name} Device = {device.displayName}");
        }

        Debug.Log($"----------------------------");
    }
}
