using System.Text.RegularExpressions;

namespace secretary.documents.creators;

public class TimeOffData 
{
    public string Name { get; set; }
    public string JobTitle { get; set; }
    public string Period { get; set; }
    public string Reason { get; set; }
    public string WorkingOff { get; set; }

    public string PeriodYear
    {
        get
        {
            if (this.Period == null)
            {
                return null;
            }
            
            var regex = new Regex(@"\d{2}\.\d{2}\.\d{2,4}");

            return regex.Match(this.Period).ToString();
        }
    }
}