using System.IO;
using HtmlAgilityPack;
using Secretary.Documents.utils;

namespace Secretary.Documents.Templates.Html
{
    public class DistantMessageAdapter : MessageAdapter
    {
        public DistantMessageAdapter(HtmlDocument doc) : base(doc)
        {
        }

        public void SetTimeOffPeriod(string value)
        {
            value = new InsertStringFormatter().Format(value);
            
            SetValue("distant_period", Placeholders.Period, value);
        }

        public void SetReason(string? value)
        {
            if (value != null)
            {
                value = new InsertStringFormatter().Format(value);
            }
            
            SetValue("distant_reason", Placeholders.Reason, value);
        }
    }
}