using Microsoft.Extensions.Logging;
using secretary.logging;
using secretary.telegram.commands.executors;
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
    
    public override async Task Execute()
    {
        await ExecuteNextState();
        await Context.SaveSession(this);
    }

    public override async Task OnMessage()
    {
        if (this.states.Count > 0)
        {
            var executor = new ChildCommandExecutor(this.states[0], Context, this);

            try
            {
                await executor.ValidateMessage();
                await executor.OnMessage();
                await ExecuteNextState();
                await Context.SaveSession(this);
            }
            catch (IncorrectFormatException e)
            {
                _logger.LogWarning(e, $"Некорректный формат команды {executor.Command.GetType().Name}: \"{Context.Message}\"");
            }
        }
    }

    public override async Task Cancel()
    {
        await base.Cancel();
        
        if (this.states.Count > 0)
        {
            var executor = new ChildCommandExecutor(this.states[0], Context, this);
            await executor.Cancel();
        }
    }

    private Task ExecuteNextState()
    {
        if (this.states.Count > 1)
        {
            this.states.RemoveAt(0);
            
            var first = states.First();

            var executor = new ChildCommandExecutor(first, Context, this);
            
            return executor.Execute();
        }
        else
        {
            return Task.CompletedTask;
        }
    }
}