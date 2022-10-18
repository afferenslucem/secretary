using Newtonsoft.Json;
using Secretary.Documents.Creators.Data;
using Secretary.Documents.Creators.DocumentCreators;
using Secretary.Documents.Creators.MessageCreators;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands.Documents.Vacation;
using Secretary.Telegram.Models;

namespace Secretary.Telegram.Commands.Caches.Documents;

public class VacationCache: 
    IVacationDocumentCache
{
    [JsonIgnore] public virtual string DocumentKey => VacationCommand.Key;
    public virtual DatePeriod? Period { get; set; }
    
    public IEnumerable<Email>? Emails { get; set; }

    public virtual string? FilePath { get; set; }

    public virtual string CreateDocument(User user)
    {
        var data = new VacationData()
        {
            Period = Period!.RawValue,
            Name = user.NameGenitive!,
            JobTitle = user.JobTitleGenitive!
        };

        var document = new VacationDocumentCreator().Create(data);

        return document;
    }

    public virtual string CreateMail(User user)
    {
        var data = new VacationData()
        {
            Period = Period!.RawValue,
            Name = user.Name!,
            JobTitle = user.JobTitle!
        };

        var document = new VacationMessageCreator().Create(data);

        return document;
    }
}