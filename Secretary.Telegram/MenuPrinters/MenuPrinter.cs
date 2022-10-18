using Secretary.Telegram.Commands;
using Telegram.Bot.Types;

namespace Secretary.Telegram.MenuPrinters;

public abstract class MenuPrinter : IMenuPrinter
{
    public async Task<Message> Print(CommandContext context)
    {   
        var menu = await CreateMenu(context);
        
        return await context.TelegramClient.SendMessage(
            context.ChatId,
            menu.Text,
            menu.Buttons
        );
    }

    public async Task Reprint(CommandContext context)
    {
        var menu = await CreateMenu(context);

        await context.TelegramClient.EditMessage(context.ChatId, context.CallbackMessageId.Value, menu.Text, menu.Buttons);
    }

    public async Task Reprint(CommandContext context, int messageId)
    {
        var menu = await CreateMenu(context);

        await context.TelegramClient.EditMessage(context.ChatId, messageId, menu.Text, menu.Buttons);
    }

    protected abstract Task<Menu> CreateMenu(CommandContext context);
}