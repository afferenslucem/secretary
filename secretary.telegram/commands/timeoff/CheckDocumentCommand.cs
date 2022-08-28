using secretary.documents.creators;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands.timeoff;

public class CheckDocumentCommand : Command
{
    public ITimeOffCreator Creator { get; set; }

    public CheckDocumentCommand()
    {
        this.Creator = new TimeOffDocumentCreator();
    }
    
    protected override async Task ExecuteRoutine()
    {
        var parent = ParentCommand as TimeOffCommand;

        var user = await Context.UserStorage.GetUser(ChatId);

        if (user == null) throw new InternalException();
        
        if (parent != null)
        {
            var data = parent.Data.ToDocumentData();

            data.Name = user.NameGenitive;
            data.JobTitle = user.JobTitleGenitive;
            
            var path = Creator.Create(data);
            parent.Data.FilePath = path;

            var fileNameParts = new[] { user.Name, data.PeriodYear, "Отгул.docx" };
            var fileName = string.Join('-', fileNameParts).Replace(' ', '-');

            
            await Context.TelegramClient.SendMessage(ChatId, "Проверьте документ");
            await Context.TelegramClient.SendDocument(ChatId, path, fileName);
            await Context.TelegramClient.SendMessageWithKeyBoard(ChatId, "Отправить заявление?", new [] {"Да", "Нет"});
        }
    }
}