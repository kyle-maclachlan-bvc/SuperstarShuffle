using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SetupStartingCoinsState : SetupState
{
    private enum Phase
    {
        IntroDialogue,
        GivingCoins,
        FinalDialogue
    }

    private Phase phase;

    public SetupStartingCoinsState(GameSetupManager setup) : base(setup)
    {
    }

    public override void Enter()
    {
        phase = Phase.IntroDialogue;
        GameEvents.OnDialogueRequested?.Invoke("Mine Foreman", new string[]
        {
            "Every explorer needs a little spending money before heading into the mines. \nTake these 10 coins."
        });
    }

    public override void DialogueFinished()
    {
        Continue();
    }

    public void Continue()
    {
        switch (phase)
        {
            case Phase.IntroDialogue:
                phase = Phase.GivingCoins;
                setup.StartCoroutine(GiveCoinsRoutine());
                break;
            case Phase.FinalDialogue:
                setup.BeginGameplay();
                break;
        }
    }

    private IEnumerator GiveCoinsRoutine()
    {
        foreach (Player player in GameManager.Instance.Players)
        {
            player.Currency.RemoveCoins(player.Currency.Coins);
        }
        
        HUDManager hud = Object.FindFirstObjectByType<HUDManager>();

        for (int i = 0; i < 10; i++)
        {
            foreach (Player player in GameManager.Instance.Players)
            {
                player.Currency.AddCoins(1);
            }

            if (hud != null)
                hud.RefreshHUD();

            yield return new WaitForSeconds(.12f);
        }
        
        phase = Phase.FinalDialogue;

        GameEvents.OnDialogueRequested?.Invoke("Mine Foreman", new string[]
        {
            "Good luck in Glitterdeep Mines!"
        });
    }
}
