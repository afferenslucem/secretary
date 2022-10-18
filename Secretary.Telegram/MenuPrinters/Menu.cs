using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.MenuPrinters;

public class Menu
{
    public string Text;
    public InlineKeyboardButton[][] Buttons;

    public Menu(string text, InlineKeyboardButton[][] buttons)
    {
        Text = text;
        Buttons = buttons;
    }
}