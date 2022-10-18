using Newtonsoft.Json;
using Secretary.Logging;
using Secretary.Telegram.Commands.Executors;
using Secretary.Telegram.Exceptions;
using Serilog;

namespace Secretary.Telegram.Commands.Abstractions;

public class CommandClip
{
    private readonly ILogger _logger = LogPoint.GetLogger<CommandClip>();
    
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

    public CommandClip(IEnumerable<Command> states)
    {
        _states = states.ToArray();
    }

    public async Task Run(CommandContext context)
    {
        if (IsFinishedChain) return;
            
        var firstPartCommand = _states[_runIndex];
            
        try
        {
            var firstPartExecutor = new CommandExecutor(firstPartCommand, context);

            await firstPartExecutor.ValidateMessage();
            var increment = await firstPartExecutor.OnMessage();
            
            IncrementStep(increment);

            if (IsFinishedChain) return;

            var secondPartCommand = _states[_runIndex];
            var secondPartExecutor = new CommandExecutor(secondPartCommand, context);

            _logger.Information($"{context.ChatId}: Start execute command {secondPartCommand.GetType().Name}");
            await secondPartExecutor.Execute();
        }
        catch (IncorrectMessageException e)
        {
            _logger.Warning(e, $"Incorrect format of command {firstPartCommand.GetType().Name}: \"{context.Message}\"");
        }
    }

    public Task Cancel(CommandContext context)
    {
        var state = _states[_runIndex];
        var executor = new CommandExecutor(state, context);

        return executor.Cancel();
    }
    
    private void IncrementStep(int value)
    {
        _runIndex += value;
    }
}