using Secretary.Telegram.Commands.Abstractions;

namespace Secretary.Telegram.Commands.Executors;

public class CommandExecutor
{
    public readonly Command Command;

    public CommandExecutor(Command command, CommandContext context)
    {
        Command = command;

        Command.Context = context;
    }

    public Task Execute()
    {
        return Command.Execute();
    }
    
    public Task<int> OnMessage()
    {
        return Command.OnMessage();
    }
    
    public Task ValidateMessage()
    {
        return Command.ValidateMessage();
    }
    
    public Task Cancel()
    {
        return Command.Cancel();
    }
    
    public Task OnForceComplete()
    {
        return Command.OnForceComplete();
    }
    
    public Task OnComplete()
    {
        return Command.OnComplete();
    }
}