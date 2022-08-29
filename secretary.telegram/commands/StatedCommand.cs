namespace secretary.telegram.commands;

public abstract class StatedCommand: Command {
    protected List<Command> states;

    protected StatedCommand() : base()
    {
        this.states = this.ConfigureStates();
    }
    
    public abstract List<Command> ConfigureStates();
    
    protected override async Task ExecuteRoutine()
    {
        await ExecuteNextState(Context);
        await Context.SaveSession(this);
    }

    protected override async Task OnMessageRoutine()
    {
        if (this.states.Count > 0)
        {
            var toRun = this.states[0];
            await toRun.OnMessage(Context, this);
            await ExecuteNextState(Context);
            await Context.SaveSession(this);
        }
    }

    public override void Cancel()
    {
        base.Cancel();
        
        if (this.states.Count > 0)
        {
            var toRun = this.states[0];
            toRun.Cancel();
        }
    }

    private Task ExecuteNextState(CommandContext context)
    {
        if (this.states.Count > 1)
        {
            this.states.RemoveAt(0);

            var first = states.First();
            
            return first.Execute(context, this);
        }
        else
        {
            return Task.CompletedTask;
        }
    }
}