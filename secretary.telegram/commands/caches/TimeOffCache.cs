using secretary.documents.creators;
using secretary.storage.models;
using secretary.telegram.models;

namespace secretary.telegram.commands.caches;

public class TimeOffCache: IEquatable<TimeOffCache>
{
    public DatePeriod? Period { get; set; }
    public string? Reason { get; set; }
    public string? WorkingOff { get; set; }
    
    public IEnumerable<Email>? Emails { get; set; }

    public string? FilePath { get; set; }

    public TimeOffData ToDocumentData()
    {
        return new TimeOffData()
        {
            Period = this.Period!.RawValue,
            Reason = this.Reason,
            WorkingOff = this.WorkingOff,
        };
    }

    public bool Equals(TimeOffCache? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        if (Emails == null && other.Emails != null) return false;
        if (Emails != null && other.Emails == null) return false;
        if (Period == null && other.Period != null) return false;
        if (Period != null && other.Period == null) return false;
        
        return (Period == other.Period || Period.Equals(other.Period)) 
               && Reason == other.Reason 
               && WorkingOff == other.WorkingOff 
               && (Emails == other.Emails || Emails!.SequenceEqual(other.Emails!))
               && FilePath == other.FilePath;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TimeOffCache)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Period, Reason, WorkingOff, Emails, FilePath);
    }
}