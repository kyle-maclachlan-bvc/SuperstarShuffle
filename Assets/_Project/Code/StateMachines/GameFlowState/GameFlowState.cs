using UnityEngine;

public abstract class GameFlowState : State
{
    protected GameManager game;

    protected GameFlowState(GameManager game)
    {
        this.game = game;
    }
}
