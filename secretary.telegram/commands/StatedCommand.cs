using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using secretary.logging;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands;

public abstract class StatedCommand: Command
{
    private ILogger<StatedCommand> _logger = LogPoint.GetLogger<StatedCommand>();

    [JsonProperty]
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

            if (Clip.IsCompleted)
            {
                await OnComplete();
            }
            else
            {
                await Context.SaveSession(this);
            }
        }
        catch (ForceCompleteCommandException e)
        {
            await this.OnForceComplete(e);
        }
    }

    public override async Task<int> OnMessage()
    {
        await this.Execute();
        return ExecuteDirection.RunNext;
    }

    public override async Task Cancel()
    {
        await base.Cancel();
        
        await Clip.Cancel(Context);
    }

    protected async Task OnComplete()
    {
        await Context.SessionStorage.DeleteSession(ChatId);
    }

    private async Task OnForceComplete(ForceCompleteCommandException e)
    {
        await this.OnComplete();
        _logger.LogWarning($"Сommand {e.CommandName} force completed");
    }
}