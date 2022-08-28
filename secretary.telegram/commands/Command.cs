using secretary.mail;
using secretary.storage;

namespace secretary.telegram.commands;

public abstract class Command
{
    public CommandContext Context { get; set; }

    protected long ChatId => Context.ChatId;

    protected string Message => Context.Message;
    
    public Command? ParentCommand { get; set; }

    public Task Execute(CommandContext context, Command? parentCommand = null)
    {
        this.Context = context;
        ParentCommand = parentCommand;

        return this.ExecuteRoutine();
    }

    protected abstract Task ExecuteRoutine();

    public Task OnMessage(CommandContext context, Command? parentCommand = null)
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
}