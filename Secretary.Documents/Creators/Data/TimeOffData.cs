namespace Secretary.Documents.Creators.Data;

public class TimeOffData : DocumentData
{
    public string Name { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public string Period { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public string WorkingOff { get; set; } = null!;
}