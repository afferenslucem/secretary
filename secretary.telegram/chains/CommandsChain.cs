using secretary.telegram.commands;
using secretary.telegram.commands.factories;
using secretary.telegram.commands.factories;

namespace secretary.telegram.chains;

class Link
{
    public readonly string Key;
    public readonly ICommandFactory CommandFactory;

    public Link(string key, ICommandFactory commandFactory)
    {
        Key = key;
        CommandFactory = commandFactory;
    }
}

public class CommandsChain
{
    private List<Link> links = new List<Link>();

    public CommandsChain()
    {
    }

    public void Add(string key, ICommandFactory commandFactory)
    {
        this.links.Add(new Link(key, commandFactory));
    }

    public Command? Get(string key)
    {
        var factory = this.links.Find(link => link.Key == key || link.Key == "*")?.CommandFactory;

        return factory?.GetCommand();
    }
}