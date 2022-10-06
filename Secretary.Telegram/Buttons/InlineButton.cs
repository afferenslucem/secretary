namespace Secretary.Telegram.Buttons;

public class InlineButton
{
    public string Text;
    public string Command;

    public InlineButton(string text, string command)
    {
        Text = text;
        Command = command;
    }
}