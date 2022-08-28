namespace secretary.telegram.commands;

public abstract class ChildCommand<TParent>: Command where TParent: StatedCommand
{
    protected TParent Parent { get; private set; }

    protected ChildCommand(TParent parent)
    {
        Parent = parent;
    }
}