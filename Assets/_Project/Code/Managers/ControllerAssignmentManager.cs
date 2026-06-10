using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerAssignmentManager : MonoBehaviour
{
    [SerializeField] private PlayerController player1;
    [SerializeField] private PlayerController player2;
    [SerializeField] private PlayerController player3;
    [SerializeField] private PlayerController player4;

    private void Start()
    {
        var gamepads = Gamepad.all;
        
        if (gamepads.Count > 0)
            player1.PairDevice(gamepads[0]);
        
        if (gamepads.Count > 1)
            player2.PairDevice(gamepads[1]);
        
        if (gamepads.Count > 2)
            player3.PairDevice(gamepads[2]);
        
        if (gamepads.Count > 3)
            player4.PairDevice(gamepads[3]);
    }
}
