
using Newtonsoft.Json;
using Secretary.Logging;
using Secretary.Telegram.Commands.ExceptionHandlers;
using Secretary.Telegram.Exceptions;
using Serilog;

namespace Secretary.Telegram.Commands;

public abstract class StatedCommand: Command
{
    private ILogger _logger = LogPoint.GetLogger<StatedCommand>();

    [JsonProperty]
    protected CommandClip Clip;

    [JsonIgnore]
    public virtual bool IsCompleted => Clip.IsCompleted;

    protected StatedCommand()
    {
        var states = ConfigureStates();
        Clip = new CommandClip(states);
    }
    
    public abstract List<Command> ConfigureStates();
    
    public override async Task Execute()
    {
        try
        {
            await Clip.Run(Context);
            if (!Clip.IsCompleted)
            {
                await SessionStorage.SaveSession(this);
            }
        }
        catch (ForceCompleteCommandException e)
        {
            await OnForceComplete();
            _logger.Warning($"Сommand {e.CommandName} force completed");
        }
    }

    public override async Task<int> OnMessage()
    {
        await Execute();
        return ExecuteDirection.RunNext;
    }

    public override async Task Cancel()
    {
        await base.Cancel();
        
        await Clip.Cancel(Context);
    }

    public override async Task OnComplete()
    {
        if (IsCompleted)
        {
            _logger.Debug($"{GetType().Name} completed");
            await base.OnComplete();
        }
    }
}