using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Factories;

namespace Secretary.Telegram.chains;

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
    private readonly List<Link> _links = new();

    public void Add(string key, ICommandFactory commandFactory)
    {
        this._links.Add(new Link(key, commandFactory));
    }

    public Command? Get(string key)
    {
        var factory = this._links.Find(link => link.Key == key || link.Key == "*")?.CommandFactory;

        return factory?.GetCommand();
    }
}