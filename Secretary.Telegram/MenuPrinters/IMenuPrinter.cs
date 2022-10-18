using Secretary.Telegram.Commands;
using Telegram.Bot.Types;

namespace Secretary.Telegram.MenuPrinters;

public interface IMenuPrinter
{
    Task<Message> Print(CommandContext context);
    Task Reprint(CommandContext context);
}