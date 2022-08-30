namespace secretary.telegram.commands;

public abstract class Command
{
    protected CancellationTokenSource CancellationToken;

    public CommandContext Context { get; set; } = null!;

    protected long ChatId => Context.ChatId;

    protected string Message => Context.Message;

    public Command? ParentCommand { get; set; } = null!;

    protected Command()
    {
        CancellationToken = new CancellationTokenSource();
    }

    public Task Execute(CommandContext context, Command? parentCommand = null)
    {
        this.Context = context;
        ParentCommand = parentCommand;

        return this.ExecuteRoutine();
    }

    protected abstract Task ExecuteRoutine();

    public virtual Task OnMessage(CommandContext context, Command? parentCommand = null)
    {
        this.Context = context;
        ParentCommand = parentCommand;
        
        return this.OnMessageRoutine();
    }

    protected internal  void ValidateMessage(string message)
    {
        
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