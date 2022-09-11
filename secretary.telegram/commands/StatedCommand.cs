﻿
using Newtonsoft.Json;
using secretary.logging;
using secretary.telegram.exceptions;
using Serilog;

namespace secretary.telegram.commands;

public abstract class StatedCommand: Command
{
    private ILogger _logger = LogPoint.GetLogger<StatedCommand>();

    [JsonProperty]
    protected CommandClip Clip;

    protected StatedCommand()
    {
        var states = this.ConfigureStates();
        this.Clip = new CommandClip(states);
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

    public override async Task OnComplete()
    {
        if (Clip.IsCompleted)
        {
            await base.OnComplete();
        }
    }

    protected async Task OnForceComplete(ForceCompleteCommandException e)
    {
        await base.OnComplete();
        _logger.Warning($"Сommand {e.CommandName} force completed");
    }
}