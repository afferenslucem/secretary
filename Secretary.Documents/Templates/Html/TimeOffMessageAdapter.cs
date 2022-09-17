using System.IO;
using HtmlAgilityPack;
using Secretary.Documents.utils;

namespace Secretary.Documents.Templates.Html
{
    public class TimeOffMessageAdapter : MessageAdapter
    {
        public TimeOffMessageAdapter(HtmlDocument doc) : base(doc)
        {
        }

        public void SetTimeOffPeriod(string value)
        {
            value = new InsertStringFormatter().Format(value);
            
            SetValue("time-off_period", Placeholders.Period, value);
        }

        public void SetReason(string? value)
        {
            if (value != null)
            {
                value = new InsertStringFormatter().Format(value);
            }
            
            SetValue("time-off_reason", Placeholders.Reason, value);
        }

        public void SetWorkingOff(string? value)
        {
            if (value != null)
            {
                value = new InsertStringFormatter().Format(value, firstLetter: FirstLetter.Upper);
            }
            
            SetValue("time-off_working-off", Placeholders.WorkingOff, value);
        }
    }
}