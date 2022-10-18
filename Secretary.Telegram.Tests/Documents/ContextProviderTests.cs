using Secretary.Telegram.Commands.Documents.Distant;
using Secretary.Telegram.Commands.Documents.TimeOff;
using Secretary.Telegram.Commands.Documents.Vacation;
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
    
    [Test]
    public void ShouldReturnDistantContext()
    {
        var context = DocumentContextProvider.GetContext(DistantCommand.Key);
        
        Assert.That(context.DisplayName, Is.EqualTo("Заявление на удаленную работу.docx"));
        Assert.That(context.MailTheme, Is.EqualTo("Удаленная работа"));
        Assert.That(context.Key, Is.EqualTo(DistantCommand.Key));
    }
}
