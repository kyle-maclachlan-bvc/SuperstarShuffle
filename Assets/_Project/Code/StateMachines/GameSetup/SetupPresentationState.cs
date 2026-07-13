using System.Collections.Generic;
using UnityEngine;

public class SetupPresentationState : SetupState
{
    private Queue<Player> presentationQueue = new ();
    private Player currentPlayer;
    
    public SetupPresentationState(GameSetupManager setup) : base(setup)
    {
    }

    public override void Enter()
    {
        presentationQueue.Clear();

        foreach (Player player in GameManager.Instance.TurnOrder)
            presentationQueue.Enqueue(player);

        ShowNextPlayer();
    }
    
    public override void DialogueFinished()
    {
        ContinuePresentation();
    }

    private void ShowNextPlayer()
    {
        if (presentationQueue.Count == 0)
        {
            setup.EnterStartingCoinsState();
            return;
        }
        
        currentPlayer = presentationQueue.Dequeue();

        int placement = currentPlayer.Data.TurnOrderPosition + 1;
        string suffix = GetOrdinal(placement);
        
        GameEvents.OnDialogueRequested?.Invoke("Mine Foreman", new string[]
        {
            $"{currentPlayer.Data.PlayerName} goes {suffix}."
        });
    }

    public void ContinuePresentation()
    {
        GameEvents.OnTurnOrderConfirmed?.Invoke(currentPlayer);
        ShowNextPlayer();
    }

    private string GetOrdinal(int number)
    {
        switch (number)
        {
            case 1: return "first";
            case 2: return "second";
            case 3: return "third";
            case 4: return "last";
        }
        
        return number.ToString();
    }
}
