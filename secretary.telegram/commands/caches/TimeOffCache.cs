using secretary.documents.creators;
using secretary.storage.models;

namespace secretary.telegram.commands.caches;

public class TimeOffCache 
{
    public string? Period { get; set; }
    public string? Reason { get; set; }
    public string? WorkingOff { get; set; }
    
    public IEnumerable<Email> receivers { get; set; }

    public string? FilePath { get; set; }

    public TimeOffData ToDocumentData()
    {
        return new TimeOffData()
        {
            Period = this.Period,
            Reason = this.Reason,
            WorkingOff = this.WorkingOff,
        };
    }
}