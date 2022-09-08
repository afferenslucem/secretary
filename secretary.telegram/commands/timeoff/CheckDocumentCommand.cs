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

        var fileNameParts = new[] { user.Name, cache.Period!.DayPeriod.Replace(" ", ""), "Отгул.docx" };
        var fileName = string.Join('-', fileNameParts).Replace(' ', '-');

        await CacheService.SaveEntity(cache);
        await TelegramClient.SendMessage("Проверьте документ");
        await TelegramClient.SendDocument(path, fileName);
        await TelegramClient.SendMessageWithKeyBoard("Отправить заявление?", new [] {"Да", "Нет"});
    }
}