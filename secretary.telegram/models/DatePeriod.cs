namespace secretary.telegram.models;

public class DatePeriod: IEquatable<DatePeriod>
{
    public DateTime StartDate { get; init; }
    public DateTime FinishDate { get; init; }
    public string RawValue { get; init; }
    
    public DatePeriod(DateTime startDate, DateTime finishDate, string rawValue)
    {
        StartDate = startDate;
        FinishDate = finishDate;
        RawValue = rawValue;
    }

    public override string ToString()
    {
        return $"с {StartDate} до {FinishDate}";
    }

    public bool Equals(DatePeriod? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return StartDate.Equals(other.StartDate) && FinishDate.Equals(other.FinishDate) && RawValue == other.RawValue;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((DatePeriod)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StartDate, FinishDate, RawValue);
    }
}