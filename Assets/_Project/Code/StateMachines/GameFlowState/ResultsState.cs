using UnityEngine;

public class ResultsState : GameFlowState
{
    public ResultsState(GameManager game) : base(game)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entered Results");

        GameEvents.OnResultsStarted?.Invoke();
    }

    public override void Exit()
    {
        Debug.Log("Leaving Results");
    }
}