using secretary.storage.models;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands;

public class RegisterUserCommand: StatedCommand
{
    public const string Key = "/registeruser";
    
    public override List<Command> ConfigureStates()
    {
        return new List<Command>
        {
            new EmptyCommand(),
            new EnterNameCommand(),
            new EnterNameGenitiveCommand(),
            new EnterJobTitleCommand(),
            new EnterJobTitleGenitiveCommand(),
        };
    }
}

class EnterNameCommand : Command
{
    protected override Task ExecuteRoutine()
    {
        return Context.TelegramClient.SendMessage(ChatId, "Введите ваши имя и фамилию в именительном падеже.\r\n" +
                                                          "Так они будут указаны в почтовом ящике, с которого будет отправляться письмо.\r\n" +
                                                          @"Например: <i>Александр Пушкин</i>");
    }

    protected override async Task OnMessageRoutine()
    {
        var user = await Context.UserStorage.GetUser(ChatId);

        user = user ?? new User()
        {
            ChatId = ChatId,
        };

        user.Name = Message;

        await Context.UserStorage.SetUser(user);
    }
}

class EnterNameGenitiveCommand : Command
{
    protected override Task ExecuteRoutine()
    {
        return Context.TelegramClient.SendMessage(ChatId, "Введите ваши имя и фамилию в родительном падеже.\r\n" +
                                                          "Так они будут указаны в отправоляемом документе в графе \"от кого\".\r\n" +
                                                          @"Например: От <i>Пушкина Александра Сергеевича</i>");
    }

    protected override async Task OnMessageRoutine()
    {
        var user = await Context.UserStorage.GetUser(ChatId);

        if (user == null) throw new InternalException();
        
        user.NameGenitive = Message;

        await Context.UserStorage.UpdateUser(user);
    }
}

class EnterJobTitleCommand : Command
{
    protected override Task ExecuteRoutine()
    {
        return Context.TelegramClient.SendMessage(ChatId, "Введите вашу должность в именительном падеже.\r\n" +
                                                          "Так она будут указана в подписи письма.\r\n" +
                                                          @"Например: С уважением, <i>поэт</i> Александр Пушкин");
    }

    protected override async Task OnMessageRoutine()
    {
        var user = await Context.UserStorage.GetUser(ChatId);

        if (user == null) throw new InternalException();
        
        user.JobTitle = Message;

        await Context.UserStorage.UpdateUser(user);
    }
}

class EnterJobTitleGenitiveCommand : Command
{
    protected override Task ExecuteRoutine()
    {
        return Context.TelegramClient.SendMessage(ChatId, "Введите вашу должность в родительном падеже.\r\n" +
                                                          "Так она будут указана в графе \"от кого\".\r\n" +
                                                          @"Например: От <i>поэта</i> Пушкина Александра Сергеевича");
    }

    protected override async Task OnMessageRoutine()
    {
        var user = await Context.UserStorage.GetUser(ChatId);

        if (user == null) throw new InternalException();
        
        user.JobTitleGenitive = Message;

        await Context.UserStorage.UpdateUser(user);

        await Context.TelegramClient.SendMessage(ChatId, "Ваш пользователь успешно сохранен");
    }
}