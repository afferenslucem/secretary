using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Exceptions;
using Secretary.Telegram.MenuPrinters;

namespace Secretary.Telegram.Commands.Menus;

public class JiraMenuCommand: Command
{
    public IMenuPrinter MenuPrinter;
    
    public const string Key = "/jira";

    public JiraMenuCommand()
    {
        MenuPrinter = new JiraMenuPrinter();
    }
    
    public override async Task Execute()
    {
        try
        {
            await ValidateUser();
            
            if (Context.UserMessage.CallbackMessageId.HasValue)
            {
                await MenuPrinter.Reprint(Context);
            }
            else
            {
                await MenuPrinter.Print(Context);
            }
        }
        catch (JiraException e)
        {
            if (e.Message == "Empty token")
            {
                await TelegramClient.SendMessage(
                    "У вас отсутствует PAT для JIRA\nВыполните команду /registerjiratoken");
            }
        }
    }

    public async Task ValidateUser()
    {
        var user = await UserStorage.GetUser();

        if (user?.JiraPersonalAccessToken == null)
        {
            throw new JiraException("Empty token");
        }
    }
}