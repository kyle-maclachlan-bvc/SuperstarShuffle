using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the project's Input Actions asset.
///
/// Responsibilities:
/// - Creates the MarioPartyControls input asset (named because the controls are designed to be similar to Mario Party, as opposed to other games)
/// - Enable input when the game starts.
/// - Disable input when the object is disabled.
/// - Provide centralized access to input actions
///
/// This manager acts as a single access point for all player input throughout the project.
/// Other systems can access input actions through InputManager.Instance.Control
/// </summary>

public class InputManager : Singleton<InputManager>
{
    private MarioPartyControls controls; // The generated Input Actions asset created from MarioPartyControls.inputactions.

    public MarioPartyControls Controls => controls; // provides read-only access to the active Input Actions asset

    private readonly Dictionary<int, Gamepad> playerGamepads = new();
    
    // Initializes the Input Manager singleton and creates the Input Actions asset.
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        controls = new MarioPartyControls();
    }

    private void Start()
    {
        AssignControllers();
    }

    private void AssignControllers()
    {
        playerGamepads.Clear();

        for (int i = 0; i < Gamepad.all.Count && i < 4; i++)
        {
            playerGamepads.Add(i + 1, Gamepad.all[i]);
            Debug.Log($"Player {i + 1} assigned to {Gamepad.all[i].displayName}");
        }
    }

    public Gamepad GetGamepad(int playerID)
    {
        if (playerGamepads.TryGetValue(playerID, out Gamepad gamepad))
            return gamepad;

        return null;
    }
    
    // Enables all input action maps when InputManager becomes active.
    private void OnEnable()
    {
        controls.Enable();
    }

    // Disables all input action maps when the InputManager becomes inactive
    private void OnDisable()
    {
        if (controls != null)
            controls.Disable();
    }

    public void PlayerRollPressed(int playerID)
    {
        
    }

    /*public bool PlayerRollPressed(int playerID)
    {
        Gamepad gamepad = GetGamepad(playerID);
        bool controllerPressed = gamepad != null && gamepad.buttonSouth.wasPressedThisFrame;

        switch (playerID)
        {
            case 1:
            {
                bool pressed = Keyboard.current.qKey.wasPressedThisFrame || controllerPressed;
                if (pressed)
                    Debug.Log("Player 1 Pressed");
                return pressed;
            }
            case 2:
            {
                bool pressed = Keyboard.current.wKey.wasPressedThisFrame || controllerPressed;
                if (pressed)
                    Debug.Log("Player 2 Pressed");
                return pressed;
            }
            case 3:
            {
                bool pressed = Keyboard.current.eKey.wasPressedThisFrame || controllerPressed;
                if (pressed)
                    Debug.Log("Player 3 Pressed");
                return pressed;
            }
            case 4:
            {
                bool pressed = Keyboard.current.rKey.wasPressedThisFrame || controllerPressed;
                if (pressed)
                    Debug.Log("Player 4 Pressed");
                return pressed;
            }
        }

        return false;

    }*/
}
