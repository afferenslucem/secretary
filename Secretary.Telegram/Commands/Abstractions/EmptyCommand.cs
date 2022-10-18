namespace Secretary.Telegram.Commands.Abstractions;

public class EmptyCommand: Command
{
    public const string Key = "/empty";
    
    public override Task Execute()
    {
        return Task.CompletedTask;
    }

    public override Task OnComplete()
    {
        return Task.CompletedTask;
    }

    public override Task<int> OnMessage()
    {
        return base.OnMessage();
    }
}