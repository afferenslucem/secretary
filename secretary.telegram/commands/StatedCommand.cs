using Microsoft.Extensions.Logging;
using secretary.logging;

namespace secretary.telegram.commands;

public abstract class StatedCommand: Command
{
    private ILogger<StatedCommand> _logger = LogPoint.GetLogger<StatedCommand>();

    protected CommandClip Clip;

    protected StatedCommand() : base()
    {
        var states = this.ConfigureStates();
        this.Clip = new CommandClip(states, this);
    }
    
    public abstract List<Command> ConfigureStates();
    
    public override async Task Execute()
    {
        await Clip.Run(Context);
        await Context.SaveSession(this);
    }

    public override async Task<int> OnMessage() {
        await Clip.Run(Context);
        await Context.SaveSession(this);
        
        return RunNext;
    }

    public override async Task Cancel()
    {
        await base.Cancel();
        
        await Clip.Cancel(Context);
    }

    public override async Task OnComplete()
    {
        if (Clip.IsFinished)
        {
            await base.OnComplete();
        }
    }
}