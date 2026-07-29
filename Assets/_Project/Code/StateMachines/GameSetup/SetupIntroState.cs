public class SetupIntroState : SetupState
{
    public SetupIntroState(GameSetupManager setup) : base(setup)
    {
    }

    public override void Enter()
    {
        GameEvents.OnDialogueRequested?.Invoke(
            "Mine Foreman", new string[]
            {
                "Welcome to Glitterdeep Mines!",
                "Deep beneath the mountain lies one of the oldest mining operations, famous for its crystal caverns.",
                "For generations, miners searched these caves for rare gems and precious ores.",
                "Although much of the mine has been abandoned, discoveries can still be found in these chambers.",
                "Before the expedition begins, we'll determine the order in which everyone takes their turns."
            });
    }

    public override void DialogueFinished()
    {
        setup.EnterRollState();
    }
}