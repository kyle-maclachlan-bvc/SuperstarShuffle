using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class GameSetupState : GameFlowState
{
    public GameSetupState(GameManager game) : base(game)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entered Game Setup State");
        GameEvents.OnGameStateChanged?.Invoke(this);
        game.GameSetupManager.StartGameSetup();
    }

    public override void Exit()
    {
        Debug.Log("Exiting Game Setup");
    }
}
