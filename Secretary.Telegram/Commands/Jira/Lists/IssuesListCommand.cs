using Secretary.JiraManager;
using Secretary.JiraManager.Data;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Caches.Jira;
using Secretary.Telegram.Commands.Formatters;
using Telegram.Bot.Types;

namespace Secretary.Telegram.Commands.Jira.Lists;

public abstract class IssuesListCommand<TCache>: Command
    where TCache: IssuesListNavigationCache, new()
{
    public IJiraReporterFactory JiraReporterFactory;

    public IssuesListCommand(IJiraReporterFactory jiraReporterFactory)
    {
        JiraReporterFactory = jiraReporterFactory;
    }
    
    public override async Task Execute()
    {
        if (Context.UserMessage.HasArguments)
        {
            await HandleCallback();
        }
        else
        {
            await HandleCommand();
        }
    }

    public async Task HandleCallback()
    {
        var argument = Context.UserMessage.CommandArgument;
        
        int page;
        
        if (int.TryParse(argument, out page))
        {
            await RewritePage(page);
        }
    }

    public async Task RewritePage(int page)
    {
        var issuesPage = await GetIssues(page);

        var cache = await CacheService.GetEntity<TCache>();

        if (cache.MessageIds.Count == issuesPage.Data.Count())
        {
            for (var i = 0; i < issuesPage.Data.Count(); i++)
            {
                var issueInfo = issuesPage.Data.ElementAt(i);
                var messageId = cache.MessageIds[i];
                
                var menu = new IssueFormatter().GetData(issueInfo);
                await TelegramClient.EditMessage(messageId, menu.Text, menu.Buttons);
            }
            
            await ReprintNavigation(page, 5, issuesPage.TotalIssues, cache.MenuMessageId);
        }

        if (cache.MessageIds.Count > issuesPage.Data.Count())
        {
            var messages = cache.MessageIds.TakeLast(issuesPage.Data.Count());

            var deletingMessages = cache.MessageIds.SkipLast(messages.Count());
            await Task.WhenAll(deletingMessages.Select(message => TelegramClient.DeleteMessage(message)));
            
            for (var i = 0; i < issuesPage.Data.Count(); i++)
            {
                var issueInfo = issuesPage.Data.ElementAt(i);
                var messageId = messages.ElementAt(i);
                
                var menu = new IssueFormatter().GetData(issueInfo);
                await TelegramClient.EditMessage(messageId, menu.Text, menu.Buttons);
            }

            await ReprintNavigation(page, 5, issuesPage.TotalIssues, cache.MenuMessageId);
            
            cache.MessageIds = new List<int>(messages);

            await CacheService.SaveEntity(cache);
        }

        if (cache.MessageIds.Count < issuesPage.Data.Count())
        {
            var messages = cache.MessageIds.Append(cache.MenuMessageId);
            var needAdd = issuesPage.Data.Count() - messages.Count();
            
            for (var i = 0; i < messages.Count(); i++)
            {
                var issueInfo = issuesPage.Data.ElementAt(i);
                var messageId = messages.ElementAt(i);
                
                var menu = new IssueFormatter().GetData(issueInfo);
                await TelegramClient.EditMessage(messageId, menu.Text, menu.Buttons);
            }

            cache.MessageIds = new List<int>(messages);
            
            for (var i = 0; i < needAdd; i++)
            {
                var issueInfo = issuesPage.Data.ElementAt(i + messages.Count());
                
                var menu = new IssueFormatter().GetData(issueInfo);
                var message = await TelegramClient.SendMessage(menu.Text, menu.Buttons);
                
                cache.MessageIds.Add(message.MessageId);
            }
            
            var menuMessage = await PrintNavigation(page, 5, issuesPage.TotalIssues);
            cache.MenuMessageId = menuMessage.MessageId;
        
            await CacheService.SaveEntity(cache);
        }
    }

    public async Task HandleCommand()
    {
        var issuesPage = await GetIssues();

        await TelegramClient.SendMessage(GetListHeader());
        
        if (issuesPage.Data.Count() == 0)
        {
            await TelegramClient.SendMessage("Здесь ничего нет");
            return;
        }

        var cache = new TCache();

        foreach (var issueInfo in issuesPage.Data)
        {
            var menu = new IssueFormatter().GetData(issueInfo);
            var message = await TelegramClient.SendMessage(menu.Text, menu.Buttons);
            cache.MessageIds.Add(message.MessageId);
        }

        var menuMessage = await PrintNavigation(1, 5, issuesPage.TotalIssues);
        cache.MenuMessageId = menuMessage.MessageId;
        
        await CacheService.SaveEntity(cache);
    }

    public abstract Task<Page<IssueInfo>> GetIssues(int page = 1, int pageLength = 5);
    
    public abstract Task<Message> PrintNavigation(int page, int pageLength, int totalIssues);
    public abstract Task ReprintNavigation(int page, int pageLength, int totalIssues, int messageId);

    public abstract string GetListHeader();
}