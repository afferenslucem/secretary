using Newtonsoft.Json;
using secretary.telegram.exceptions;
using secretary.telegram.wrappers;

namespace secretary.telegram.commands;

public abstract class Command
{
    protected readonly CancellationTokenSource CancellationToken;

    [JsonIgnore]
    public CommandContext Context { get; set; } = null!;

    protected TelegramClientWrapper TelegramClient => new (Context.TelegramClient, Context.ChatId);
    protected SessionStorageWrapper SessionStorage => new (Context.SessionStorage, Context.ChatId);
    protected UserStorageWrapper UserStorage => new (Context.UserStorage, Context.ChatId);
    protected DocumentStorageWrapper DocumentStorage => new (Context.DocumentStorage, Context.ChatId);

    protected long ChatId => Context.ChatId;

    protected string Message => Context.Message;

    protected Command()
    {
        CancellationToken = new CancellationTokenSource();
    }

    public abstract Task Execute();

    public virtual Task<int> OnMessage()
    {
        return Task.FromResult(ExecuteDirection.RunNext);
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

    protected void ForceComplete()
    {
        throw new ForceCompleteCommandException(this.GetType().Name);
    }
    
    public virtual async Task OnComplete()
    {
        await SessionStorage.DeleteSession();
    }
}