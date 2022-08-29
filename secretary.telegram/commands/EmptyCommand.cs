namespace secretary.telegram.commands;

public class EmptyCommand: Command
{
    protected override Task ExecuteRoutine()
    {
        return Task.CompletedTask;
    }
}