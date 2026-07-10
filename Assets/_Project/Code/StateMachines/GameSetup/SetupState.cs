public abstract class SetupState : State
{
    protected GameSetupManager setup;

    protected SetupState(GameSetupManager setup)
    {
        this.setup = setup;
    }
}
