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

    public Task Execute()
    {
        return this.ExecuteRoutine();
    }

    protected abstract Task ExecuteRoutine();

    public virtual Task OnMessage()
    {
        return this.OnMessageRoutine();
    }

    public virtual Task ValidateMessage()
    {
        return Task.CompletedTask;
    }
    
    protected virtual Task OnMessageRoutine()
    {
        return Task.CompletedTask;
    }

    public virtual void Cancel()
    {
        this.CancellationToken.Cancel();
    }
}