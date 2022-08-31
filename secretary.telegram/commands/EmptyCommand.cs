namespace secretary.telegram.commands;

public class EmptyCommand: Command
{
    public override Task Execute()
    {
        return Task.CompletedTask;
    }
}