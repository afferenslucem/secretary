namespace secretary.telegram.commands.caches;

public class RegisterUserCache: IEquatable<RegisterUserCache>
{
    public string? Name { get; set; }
    public string? NameGenitive { get; set; }
    public string? JobTitle { get; set; }
    public string? JobTitleGenitive { get; set; }

    public bool Equals(RegisterUserCache? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && NameGenitive == other.NameGenitive && JobTitle == other.JobTitle && JobTitleGenitive == other.JobTitleGenitive;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((RegisterUserCache)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, NameGenitive, JobTitle, JobTitleGenitive);
    }
}