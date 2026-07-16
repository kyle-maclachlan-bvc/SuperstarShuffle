public abstract class State
{
    public virtual void Enter()
    { }

    public virtual void Exit()
    { }
    
    public virtual void Tick()
    { }
    
    public virtual void FixedTick()
    { }

    public virtual void DialogueFinished()
    { }
    
}
