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
        var user = (await UserStorage.GetUser())!;
        var cache = await CacheService.GetEntity<TimeOffCache>();

        if (cache == null) throw new InternalException();
        
        var data = cache.ToDocumentData();

        data.Name = user.NameGenitive;
        data.JobTitle = user.JobTitleGenitive;
        
        var path = Creator.Create(data);
        cache.FilePath = path;

        await CacheService.SaveEntity(cache);
        await TelegramClient.SendMessage("Проверьте документ");
        await TelegramClient.SendDocument(path, "Заявление на отгул.docx");
        await TelegramClient.SendMessageWithKeyBoard("Отправить заявление?", new [] {"Да", "Нет"});
    }
}