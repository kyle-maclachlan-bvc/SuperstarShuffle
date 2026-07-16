using UnityEngine;

public class MinigameTutorialState : GameFlowState
{
    public MinigameTutorialState(GameManager game) : base(game)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entered Minigame Tutorial");

        GameEvents.OnMinigameTutorialStarted?.Invoke();

        GameEvents.OnSceneLoadRequested?.Invoke("MG_PickaxePanic");
    }

    public override void Exit()
    {
        Debug.Log("Leaving Minigame Tutorial");
    }
}