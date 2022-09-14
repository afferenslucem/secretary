using Secretary.Telegram.Commands.Caches.Interfaces;
using Secretary.Telegram.Documents;
using Secretary.Telegram.Exceptions;

namespace Secretary.Telegram.Commands.Common;

public class CheckDocumentCommand<T> : Command
    where T : class, IDocumentCreator, IDocumentKeyCache, IFilePathCache
{
    
    public override async Task Execute()
    {
        var user = (await UserStorage.GetUser())!;
        var cache = await CacheService.GetEntity<T>();

        if (cache == null) throw new InternalException();
        
        var path = cache.CreateDocument(user);
        cache.FilePath = path;

        DocumentContext documentContext = DocumentContextProvider.GetContext(cache.DocumentKey);

        await CacheService.SaveEntity(cache);
        await TelegramClient.SendMessage("Проверьте документ");
        await TelegramClient.SendDocument(path, documentContext.DisplayName);
        await TelegramClient.SendMessageWithKeyBoard("Отправить заявление?", new [] {"Да", "Нет"});
    }

    public override async Task<int> OnMessage()
    {
        if (Message.ToLower() != "да")
        {
            await this.CancelCommand();
            this.ForceComplete();
        }

        return ExecuteDirection.RunNext;
    }

    public Task CancelCommand()
    {
        return TelegramClient.SendMessage("Дальнейшее выполнение команды прервано");
    }
}