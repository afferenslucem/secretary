using Moq;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.Tests;

public class TestUtils
{
    public static ReplyKeyboardMarkup IsItSameKeyBoards(ReplyKeyboardMarkup target)
    {
        return It.Is<ReplyKeyboardMarkup>(kb => KeyboardsAreEqual(kb, target));
    }
    
    private static bool KeyboardsAreEqual(ReplyKeyboardMarkup first, ReplyKeyboardMarkup second)
    {
        if (first.Keyboard.Count() != second.Keyboard.Count()) return false;

        return first.Keyboard.Zip(second.Keyboard)
            .All(tuple => KeyBoardsAreEqual(tuple.First, tuple.Second));
    }

    private static bool KeyBoardsAreEqual(IEnumerable<KeyboardButton> first, IEnumerable<KeyboardButton> second)
    {
        if (first.Count() != second.Count()) return false;

        var zipped = first.Zip(second);
        
        return zipped.All(tuple => tuple.First.Text == tuple.Second.Text);
    }
}