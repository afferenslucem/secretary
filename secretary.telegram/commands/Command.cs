namespace secretary.telegram.commands;

public abstract class Command
{
    protected const int RunNext = 1; 
    
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

    public virtual Task<int> OnMessage()
    {
        return Task.FromResult(RunNext);
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