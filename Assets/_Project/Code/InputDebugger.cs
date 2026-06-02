using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Debug utility used to verify Input Actions.
///
/// Responsibilities:
/// - Listen for Input Action events.
/// - Print input activity to the Console
/// - Confirm that action bindings are working
/// Assist with input system testing
///
/// This script is intended for development and debugging purposes and is not required for normal gameplay functionality
/// </summary>

public class InputDebugger : MonoBehaviour
{
    private MarioPartyControls controls; // Local instance of the Input Actions asset

    // Creates the Unput actions asset when the object is initialized.
    private void Awake()
    {
        controls = new MarioPartyControls();
    }

    // Enables input actions and subscribes to input actions when the object becomes active
    private void OnEnable()
    {
        controls.Enable();

        controls.BoardGame.Roll.performed += OnRoll;
        controls.BoardGame.Confirm.performed += OnConfirm;
        controls.BoardGame.Cancel.performed += OnCancel;
        controls.BoardGame.Pause.performed += OnPause;
    }

    // Unsubscribes fron input events and disables input actions when the object becomes inactive
    private void OnDisable()
    {
        controls.BoardGame.Roll.performed -= OnRoll;
        controls.BoardGame.Confirm.performed -= OnConfirm;
        controls.BoardGame.Cancel.performed -= OnCancel;
        controls.BoardGame.Pause.performed -= OnPause;

        controls.Disable();
    }

    // Called when the Roll action is performed. Used to verify dice roll input bindings
    private void OnRoll(InputAction.CallbackContext ctx)
    {
        Debug.Log("ROLL DICE Pressed");
    }

    // Called when the Confirm action is performed. Used to verify confirmation input bindings
    private void OnConfirm(InputAction.CallbackContext ctx)
    {
        Debug.Log("CONFIRM Pressed");
    }

    // Called when the pause action is performed. Used to verify pause menu input bindings
    private void OnCancel(InputAction.CallbackContext ctx)
    {
        Debug.Log("CANCEL Pressed");
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        Debug.Log("PAUSE Pressed");
    }
}
