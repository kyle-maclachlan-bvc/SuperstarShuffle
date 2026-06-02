using UnityEngine;

/// <summary>
/// Control's the board game's camera behavior.
///
/// Responsibilities:
/// - Follow the current active player
/// maintain a consistent viewing offset
/// keep the active player centered in view.
/// Update camera position when Turn Manager changes turn.
///
/// The TurnManager assigns the active player through FocusPlayer(),
/// and the camera follows that player continuously until a new player is active.
/// </summary>

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector3 cameraOffset =
        new Vector3(0f, 5f, -5f);
    // Offset applied between the Camera and the Player. Determines viewing angle and distance

    private Transform targetPlayer;
    // The player currently being tracked by the camera. Assigned by TurnManager when a player's turn begins.

    public void FocusPlayer(Transform player)
    {
        // Assigns a new player for the camera to follow.
        // Called by Turn Manager.
        
        targetPlayer = player;
    }

    private void LateUpdate()
    {
        // Updates the camera position after all player movment has been processed for the frame.
        // LateUpdate so the camera follows the player's final position each frame, reducing jitter.
        
        if (targetPlayer == null)
            return;
        
        transform.position =
            targetPlayer.position + cameraOffset;
        // position of the camera relative to the active player
        
        transform.LookAt(targetPlayer);
        // Rotate camera so it always looks at the player.
    }
}
