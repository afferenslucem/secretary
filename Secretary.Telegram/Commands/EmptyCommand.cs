namespace Secretary.Telegram.Commands;

public class EmptyCommand: Command
{
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