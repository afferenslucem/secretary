namespace Secretary.Documents.Creators.Data;

public class TimeOffData : DocumentData
{
    public string Name { get; set; }
    public string JobTitle { get; set; }
    public string Period { get; set; }
    public string Reason { get; set; }
    public string WorkingOff { get; set; }
}