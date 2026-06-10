public class WaitingState : PlayerState
{
    public WaitingState(Player player) : base(player)
    {
    }

    public override void Enter()
    {
        player.Controller.DisableControls();
    }
}
