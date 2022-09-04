using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using secretary.logging;
using secretary.telegram.commands.executors;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands;

public class CommandClip
{
    private readonly ILogger<CommandClip> _logger = LogPoint.GetLogger<CommandClip>();

    [JsonProperty]
    private readonly Command _parentCommand;
    
    [JsonProperty]
    private readonly Command[] _states;
    
    [JsonProperty]
    private int _runIndex = 0;

    public bool IsFinishedChain => _runIndex == _states.Length;

    private int LastIndex => _states.Length - 1;
    
    public bool IsAsymmetricCompleted
    {
        get
        {
            if (_runIndex != LastIndex - 1)
            {
                return false;
            }
            else
            {
                return _states[_runIndex + 1].GetType() == typeof(AssymetricCompleteCommand);
            }
        }
    }

    public bool IsCompleted => IsFinishedChain || IsAsymmetricCompleted;

    public int RunIndex => _runIndex;

    public CommandClip(IEnumerable<Command> states, Command parentCommand)
    {
        _parentCommand = parentCommand;
        _states = states.ToArray();
    }

    public async Task Run(CommandContext context)
    {
        if (IsFinishedChain) return;
            
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

            if (IsFinishedChain) return;

            var secondPartCommand = _states[_runIndex];
            var secondPartExecutor = new ChildCommandExecutor(secondPartCommand, context, _parentCommand);

            _logger.LogInformation($"{context.ChatId}: Start execute command {secondPartCommand.GetType().Name}");
            await secondPartExecutor.Execute();
        }
        catch (IncorrectFormatException e)
        {
            _logger.LogWarning(e, $"Incorrect format of command {firstPartCommand.GetType().Name}: \"{context.Message}\"");
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