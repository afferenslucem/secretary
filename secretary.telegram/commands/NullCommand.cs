namespace secretary.telegram.commands;

public class NullCommand: Command
{
    public const string Key = "*";
    
    protected override async Task ExecuteRoutine()
    {
        var session = await Context.GetSession();
        
        if (session == null || session.LastCommand == null) return;
        
        await session.LastCommand.OnMessage(Context, this);
    }
}