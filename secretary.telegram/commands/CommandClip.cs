using Microsoft.Extensions.Logging;
using secretary.logging;
using secretary.telegram.commands.executors;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands;

public class CommandClip
{
    private readonly ILogger<CommandClip> _logger = LogPoint.GetLogger<CommandClip>();

    private readonly Command _parentCommand;
    private readonly Command[] _states;
    private int _runIndex = 0;

    public bool IsFinished => _runIndex == _states.Length;

    public int RunIndex => _runIndex;

    public CommandClip(IEnumerable<Command> states, Command parentCommand)
    {
        _parentCommand = parentCommand;
        _states = states.ToArray();
    }

    public async Task Run(CommandContext context)
    {
        if (IsFinished) return;
            
        var firstPartCommand = _states[_runIndex];
            
        try
        {
            var firstPartExecutor = new ChildCommandExecutor(firstPartCommand, context, _parentCommand);

            await firstPartExecutor.ValidateMessage();
            var increment = await firstPartExecutor.OnMessage();

            if (increment <= 0)
            {
                context.BackwardRedirect = true;
            }
            
            this.IncrementStep(increment);

            if (IsFinished) return;

            var secondPartCommand = _states[_runIndex];
            var secondPartExecutor = new ChildCommandExecutor(secondPartCommand, context, _parentCommand);

            await secondPartExecutor.Execute();
        }
        catch (IncorrectFormatException e)
        {
            _logger.LogWarning(e, $"Incorrect format of command {firstPartCommand.GetType().Name}: \"{context.Message}\"");
        }
        catch (ForceCompleteCommandException e)
        {
            _logger.LogWarning($"Force completing command {e.CommandName}");
            throw;
        }
    }

    public Task Cancel(CommandContext context)
    {
        var state = _states[_runIndex];
        var executor = new ChildCommandExecutor(state, context, _parentCommand);

        return executor.Cancel();
    }
    
    private void IncrementStep(int value)
    {
        this._runIndex += value;
    }
}