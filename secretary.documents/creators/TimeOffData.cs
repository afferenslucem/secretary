using System.Text.RegularExpressions;

namespace secretary.documents.creators;

public class TimeOffData 
{
    public string Name { get; set; }
    public string JobTitle { get; set; }
    public string Period { get; set; }
    public string Reason { get; set; }
    public string WorkingOff { get; set; }
}