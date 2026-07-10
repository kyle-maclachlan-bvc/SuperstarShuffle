using UnityEngine;

public class SpaceResolutionManager : MonoBehaviour
{
    private BoardSpace pendingPropertySpace;
    private Player pendingPlayer;
    private bool waitingForPropertyDecision;
    
    private void OnEnable()
    {
        GameEvents.OnSpaceLanded += HandleSpaceLanded;
        GameEvents.OnPropertyPurchaseRequested += HandlePurchaseRequested;
        GameEvents.OnPropertyPurchased += HandlePropertyPurchased;
        GameEvents.OnRentPaid += HandleRentPaid;
    }

    private void OnDisable()
    {
        GameEvents.OnSpaceLanded -= HandleSpaceLanded;
        GameEvents.OnPropertyPurchaseRequested -= HandlePurchaseRequested;
        GameEvents.OnPropertyPurchased -= HandlePropertyPurchased;
        GameEvents.OnRentPaid -= HandleRentPaid;
    }

    private void Update()
    {
        if (waitingForPropertyDecision)
        {
            HandlePropertyDecisionInput();
        }
    }

    private void HandleSpaceLanded(Player player, BoardSpace space)
    {
        Debug.Log($"{player.Data.PlayerName} landed on Space {space.SpaceIndex}");
        GameEvents.OnSpaceResolutionStarted?.Invoke(player, space);
        ResolveProperty(player, space);
    }
    
    private void HandlePurchaseRequested(Player player, BoardSpace space)
    {
        Debug.Log($"EVENT: {player.Data.PlayerName} may purchase Space {space.SpaceIndex}");
    }

    private void HandlePropertyPurchased(Player player, BoardSpace space)
    {
        Debug.Log($"EVENT: {player.Data.PlayerName} purchased Space {space.SpaceIndex}");
    }

    private void HandleRentPaid(Player player, BoardSpace space)
    {
        Debug.Log($"EVENT: {player.Data.PlayerName} paid rent on Space {space.SpaceIndex}");
    }

    private void HandlePropertyDecisionInput()
    {
        if (!waitingForPropertyDecision)
            return;

        PlayerController controller = pendingPlayer.Controller;

        if (controller.ConfirmPressed())
        {
            AttemptPurchase(pendingPropertySpace, pendingPlayer);
            FinishPropertyDecision();
        }

        if (controller.CancelPressed())
        {
            Debug.Log("Property Purcahse Declined");
            FinishPropertyDecision();
        }
    }

    private void AttemptPurchase(BoardSpace space, Player player)
    {
        const int propertyCost = 5;

        if (player.Currency.Coins < propertyCost)
        {
            Debug.Log($"{player.name} cannot afford this property");
            return;
        }
        
        player.Currency.RemoveCoins(propertyCost);
        
        space.Owner = player.PropertyOwner;

        GameEvents.OnPropertyPurchased?.Invoke(player, space);

        Debug.Log($"{player.name} purchased Space {space.SpaceIndex}");
    }
    
    
    
    private void ResolveProperty(Player player, BoardSpace space)
    {
        if (space.SpaceType == SpaceType.Happening)
        {
            GameEvents.OnSpaceResolutionFinished?.Invoke(player, space);
            GameEvents.OnHappeningTriggered?.Invoke(player, space);
            return;
        }
        
        if (space.Owner == PropertyOwner.None)
        {
            pendingPlayer = player;
            pendingPropertySpace = space;
            waitingForPropertyDecision = true;
            
            GameEvents.OnPropertyPurchaseRequested?.Invoke(player, space);
            
            Debug.Log($"Purchase Space {space.SpaceIndex} for 5 coins?");
            return;
        }

        if (space.Owner == player.PropertyOwner)
        {
            CollectPropertyIncome(player);
            GameEvents.OnSpaceResolutionFinished?.Invoke(player, space);
            player.Movement.FinishMovement();
            return;
        }
        
        PayRent(space, player);
        GameEvents.OnSpaceResolutionFinished?.Invoke(player, space);
        player.Movement.FinishMovement();
        return;
    }

    private void CollectPropertyIncome(Player player)
    {
        const int propertyIncome = 5;
        
        player.Currency.AddCoins(propertyIncome);
        
        Debug.Log($"{player.name} landed on their own property and collected {propertyIncome}");
        Debug.Log($"{player.name}: {player.Currency.Coins} coins total");
    }

    private void PayRent(BoardSpace space, Player player)
    {
        const int rentCost = 3;

        Player owner = FindOwner(space.Owner);

        if (owner == null)
            return;

        int coinsPaid = player.Currency.RemoveCoins(rentCost);

        owner.Currency.AddCoins(coinsPaid);
        
        GameEvents.OnRentPaid?.Invoke(player, space);
        
        Debug.Log($"{player.name} paid {coinsPaid} coins to {owner.name}");
        Debug.Log($"{player.name}: {player.Currency.Coins} coins remaining");
        Debug.Log($"{owner.name}: {owner.Currency.Coins} coins total");
    }

    private Player FindOwner(PropertyOwner owner)
    {
        foreach (Player player in GameManager.Instance.Players)
        {
            if (player.PropertyOwner == owner)
                return player;
        }

        return null;
    }

    private void FinishPropertyDecision()
    {
        Player player = pendingPlayer;
        
        waitingForPropertyDecision = false;
        
        pendingPropertySpace = null;
        pendingPlayer = null;

        GameEvents.OnSpaceResolutionFinished?.Invoke(player, player.Movement.CurrentSpace);

        player.Movement.FinishMovement();
    }

    
}
