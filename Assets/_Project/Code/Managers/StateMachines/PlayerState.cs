public abstract class PlayerState
{
    protected Player player;

    protected PlayerState(Player player)
    {
        this.player = player;
    }

    public virtual void Enter() { }
    
    public virtual void Exit() { }
    
    public virtual void Update() { }
}
