using Secretary.Telegram.Commands.TimeOff;
using Secretary.Telegram.Commands.Vacation;
using Secretary.Telegram.Documents;

namespace Secretary.Telegram.Tests.Documents;

public class ContextProviderTests
{
    [Test]
    public void ShouldReturnTimeOffContext()
    {
        var context = DocumentContextProvider.GetContext(TimeOffCommand.Key);
        
        Assert.That(context.DisplayName, Is.EqualTo("Заявление на отгул.docx"));
        Assert.That(context.MailTheme, Is.EqualTo("Отгул"));
        Assert.That(context.Key, Is.EqualTo(TimeOffCommand.Key));
    }
    
    [Test]
    public void ShouldReturnVacationContext()
    {
        var context = DocumentContextProvider.GetContext(VacationCommand.Key);
        
        Assert.That(context.DisplayName, Is.EqualTo("Заявление на отпуск.docx"));
        Assert.That(context.MailTheme, Is.EqualTo("Отпуск"));
        Assert.That(context.Key, Is.EqualTo(VacationCommand.Key));
    }
}
