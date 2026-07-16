using UnityEngine;

public class GameEndState : GameFlowState
{
    public GameEndState(GameManager game) : base(game)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entered Game End");
    }
}