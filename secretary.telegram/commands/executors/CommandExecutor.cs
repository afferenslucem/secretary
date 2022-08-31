namespace secretary.telegram.commands.executors;

public class CommandExecutor
{
    public readonly Command Command;

    public CommandExecutor(Command command, CommandContext context)
    {
        Command = command;

        this.Command.Context = context;
    }

    public Task Execute()
    {
        return Command.Execute();
    }
    
    public Task OnMessage()
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
}