using secretary.documents.creators;
using secretary.telegram.commands.caches;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands.timeoff;

public class CheckDocumentCommand : Command
{
    public ITimeOffCreator Creator { get; set; }

    public CheckDocumentCommand()
    {
        this.Creator = new TimeOffDocumentCreator();
    }
    
    public override async Task Execute()
    {
        var user = (await Context.UserStorage.GetUser(ChatId))!;
        var cache = await Context.CacheService.GetEntity<TimeOffCache>(ChatId);

        if (cache == null) throw new InternalException();
        
        var data = cache.ToDocumentData();

        data.Name = user.NameGenitive;
        data.JobTitle = user.JobTitleGenitive;
        
        var path = Creator.Create(data);
        cache.FilePath = path;

        var fileNameParts = new[] { user.Name, data.PeriodYear, "Отгул.docx" };
        var fileName = string.Join('-', fileNameParts).Replace(' ', '-');

        await Context.CacheService.SaveEntity(ChatId, cache);
        await Context.TelegramClient.SendMessage(ChatId, "Проверьте документ");
        await Context.TelegramClient.SendDocument(ChatId, path, fileName);
        await Context.TelegramClient.SendMessageWithKeyBoard(ChatId, "Отправить заявление?", new [] {"Да", "Нет"});
    }
}