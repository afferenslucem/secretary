namespace secretary.telegram.commands.executors;

public class ChildCommandExecutor: CommandExecutor
{
    public ChildCommandExecutor(Command command, CommandContext context, Command parentCommand): base(command, context)
    {
        this.Command.ParentCommand = parentCommand;
    }
}