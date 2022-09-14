using HtmlAgilityPack;
using Secretary.Documents.utils;

namespace Secretary.Documents.Templates.Html
{
    public class VacationMessageAdapter : MessageAdapter
    {
        public VacationMessageAdapter(HtmlDocument doc) : base(doc)
        {
        }

        public void SetDate(string value)
        {
            value = new InsertStringFormatter().Format(value, extraPoint: false);
            
            SetValue("vacation_period", Placeholders.Period, value);
        }
    }
}