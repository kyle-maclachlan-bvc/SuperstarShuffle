using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerDebugger : MonoBehaviour
{
    private void Start()
    {
        Debug.Log($"Connected Gamepads: {Gamepad.all.Count}");

        foreach (Gamepad gamepad in Gamepad.all)
        {
            Debug.Log($"Gamepad connected: {gamepad.name}");
        }
    }

    private void Update()
    {
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            Debug.Log($"South Button Pressed");
        }
    }
}
