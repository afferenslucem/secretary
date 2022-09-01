using Microsoft.Extensions.Logging;
using secretary.logging;
using secretary.telegram.exceptions;

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
        try
        {
            await Clip.Run(Context);
            await Context.SaveSession(this);
        }
        catch (ForceCompleteCommandException e)
        {
            await base.OnComplete();
            _logger.LogWarning($"Сommand {e.CommandName} force completed");
        }
    }

    public override async Task<int> OnMessage()
    {
        await this.Execute();
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