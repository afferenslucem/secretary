using Secretary.Documents.Creators;
using Secretary.Documents.Creators.Data;
using Secretary.Documents.Creators.DocumentCreators;
using Secretary.Documents.Creators.MessageCreators;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands.TimeOff;
using Secretary.Telegram.Commands.Vacation;
using Secretary.Telegram.Models;

namespace Secretary.Telegram.Commands.Caches;

public class VacationCache: 
    IVacationDocumentCache
{
    public virtual string DocumentKey { get; set; } = VacationCommand.Key;
    public virtual DatePeriod? Period { get; set; }
    
    public IEnumerable<Email>? Emails { get; set; }

    public virtual string? FilePath { get; set; }

    public virtual string CreateDocument(User user)
    {
        var data = new VacationData()
        {
            Period = this.Period!.RawValue,
            Name = user.NameGenitive,
            JobTitle = user.JobTitleGenitive
        };

        var document = new VacationDocumentCreator().Create(data);

        return document;
    }

    public virtual string CreateMail(User user)
    {
        var data = new VacationData()
        {
            Period = Period!.RawValue,
            Name = user.Name,
            JobTitle = user.JobTitle
        };

        var document = new VacationMessageCreator().Create(data);

        return document;
    }
}