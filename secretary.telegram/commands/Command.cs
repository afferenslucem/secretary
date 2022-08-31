namespace secretary.telegram.commands;

public abstract class Command
{
    protected readonly CancellationTokenSource CancellationToken;

    public CommandContext Context { get; set; } = null!;

    protected long ChatId => Context.ChatId;

    protected string Message => Context.Message;

    public Command? ParentCommand { get; set; } = null!;

    protected Command()
    {
        CancellationToken = new CancellationTokenSource();
    }

    public abstract Task Execute();

    public virtual Task OnMessage()
    {
        return Task.CompletedTask;
    }

    public virtual Task ValidateMessage()
    {
        return Task.CompletedTask;
    }

    public virtual Task Cancel()
    {
        this.CancellationToken.Cancel();
        
        return Task.CompletedTask;
    }
}