using Microsoft.Extensions.Logging;
using secretary.logging;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands;

public abstract class StatedCommand: Command
{
    private ILogger<StatedCommand> _logger = LogPoint.GetLogger<StatedCommand>();

    protected List<Command> states;

    protected StatedCommand() : base()
    {
        this.states = this.ConfigureStates();
    }
    
    public abstract List<Command> ConfigureStates();
    
    protected override async Task ExecuteRoutine()
    {
        await ExecuteNextState();
        await Context.SaveSession(this);
    }

    protected override async Task OnMessageRoutine()
    {
        if (this.states.Count > 0)
        {
            var toRun = this.states[0];
            toRun.Context = Context;
            toRun.ParentCommand = this;

            try
            {
                await toRun.ValidateMessage();
                await toRun.OnMessage();
                await ExecuteNextState();
                await Context.SaveSession(this);
            }
            catch (IncorrectFormatException e)
            {
                _logger.LogWarning(e, $"Некорректный формат команды {toRun.GetType().Name}: \"{Context.Message}\"");
            }
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

    private Task ExecuteNextState()
    {
        if (this.states.Count > 1)
        {
            this.states.RemoveAt(0);
            
            var first = states.First();

            first.Context = Context;
            first.ParentCommand = this;
            
            return first.Execute();
        }
        else
        {
            return Task.CompletedTask;
        }
    }
}