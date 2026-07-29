using System.Collections.Generic;
using UnityEngine;

public class OrderPresentationManager : MonoBehaviour
{
    private Queue<Player> presentationQueue = new();
    private bool presenting;
    private bool waitingForDialogue;

    private Player currentPlayer;
    
    private void OnEnable()
    {
        GameEvents.OnTurnOrderPresentationRequested += BeginPresentation;
        GameEvents.OnDialogueFinished += HandleDialogueFinished;
    }

    private void OnDisable()
    {
        GameEvents.OnTurnOrderPresentationRequested -= BeginPresentation;
        GameEvents.OnDialogueFinished -= HandleDialogueFinished;
    }

    private void BeginPresentation()
    {
        presentationQueue.Clear();
        
        foreach (Player player in GameManager.Instance.TurnOrder)
            presentationQueue.Enqueue(player);

        presenting = true;

        ShowNextPlayer();
    }

    private void ShowNextPlayer()
    {
        if (presentationQueue.Count == 0)
        {
            FinishPresentation();
            return;
        }

        currentPlayer = presentationQueue.Dequeue();

        int placement = currentPlayer.Data.TurnOrderPosition + 1;
        string suffix = GetOrdinal(placement);
        
        waitingForDialogue = true;
        
        GameEvents.OnDialogueRequested?.Invoke(
            "Mine Foreman", new string[]
            {
                $"{currentPlayer.Data.PlayerName} goes {suffix}."
            });
    }

    private void HandleDialogueFinished()
    {
        if (!waitingForDialogue)
            return;
        
        waitingForDialogue = false;

        GameEvents.OnTurnOrderConfirmed?.Invoke(currentPlayer);
        
        ShowNextPlayer();
    }
    
    private void FinishPresentation()
    {
        presenting = false;
        GameEvents.OnTurnOrderPresentationFinished?.Invoke();
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
