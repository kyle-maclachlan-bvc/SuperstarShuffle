using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerDebug : MonoBehaviour
{
    private void Start()
    {
        Debug.Log($"Gamepad Count = {Gamepad.all.Count}");

        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            Debug.Log($"Gamepad {i}: {Gamepad.all[i].displayName}");
        }
    }

    private void Update()
    {
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            if (Gamepad.all[i].buttonSouth.wasPressedThisFrame)
            {
                Debug.Log($"Controller {i} South Button Pressed");
            }
        }
    }
}
