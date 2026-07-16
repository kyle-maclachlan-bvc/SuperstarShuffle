using UnityEngine;

public class MinigameState : GameFlowState
{
    public MinigameState(GameManager game) : base(game)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entered Minigame");

        GameEvents.OnMinigameStarted?.Invoke();
    }

    public override void Exit()
    {
        Debug.Log("Leaving Minigame");
    }
}