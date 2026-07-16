using UnityEngine;

public class BoardGameplayState : GameFlowState
{
    public BoardGameplayState(GameManager game) : base(game)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entered Board Gameplay");
        GameEvents.OnGameStateChanged?.Invoke(this);
    }

    public override void Exit()
    {
        Debug.Log("Leaving Board Gameplay");
    }
}
