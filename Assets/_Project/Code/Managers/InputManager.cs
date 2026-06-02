using UnityEngine;

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

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; } // Global reference to the input manager. Singleton.

    private MarioPartyControls controls; // The generated Input Actions asset created from MarioPartyControls.inputactions.

    public MarioPartyControls Controls => controls; // provides read-only access to the active Input Actions asset

    // Initializes the Input Manager singleton and creates the Input Actions asset.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        controls = new MarioPartyControls();
    }

    // Enables all input action maps when InputManager becomes active.
    private void OnEnable()
    {
        controls.Enable();
    }

    // Disables all input action maps when the InputManager becomes inactive
    private void OnDisable()
    {
        controls.Disable();
    }
}
