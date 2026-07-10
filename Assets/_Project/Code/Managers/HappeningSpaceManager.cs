using UnityEngine;

public class HappeningSpaceManager : MonoBehaviour
{
    [SerializeField] private int destinationSpaceIndex = 79;
    
    private void OnEnable()
    {
        GameEvents.OnHappeningTriggered += HandleHappening;
    }

    private void OnDisable()
    {
        GameEvents.OnHappeningTriggered -= HandleHappening;
    }

    private void HandleHappening(Player player, BoardSpace space)
    {
        Debug.Log($"{player.Data.PlayerName} triggered Happening Space {space.SpaceIndex}");
        
        // Teleport destination
        BoardSpace destination = BoardManager.Instance.GetSpace(destinationSpaceIndex);

        if (destination == null)
        {
            Debug.LogError("Destination Space 18 could not be found");
            return;
        }
        
        // Update the player's logical position
        player.Movement.CurrentSpace = destination;
        
        // move the player
        player.transform.position = destination.transform.position;
        
        // save the player's position
        player.Data.CurrentSpaceIndex = destination.SpaceIndex;
        
        Debug.Log($"{player.Data.PlayerName} moved to Space {destination.SpaceIndex}");
        
        //finish space resolution
        GameEvents.OnSpaceResolutionFinished?.Invoke(player, destination);
        
        // finish player movment
        player.Movement.FinishMovement();
    }
}
