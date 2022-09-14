using Secretary.Documents.utils;
using Xceed.Words.NET;

namespace Secretary.Documents.Templates.Docx
{
    public class TimeOffDocumentAdapter: DocumentAdapter
    {
        public TimeOffDocumentAdapter(DocX document) : base(document)
        {
        }

        public void SetPeriod(string value)
        {
            if (value != null)
            {
                value = new InsertStringFormatter().Format(value);
            }
            
            SetValue(Placeholders.Period, value);
        }

        public void SetReason(string value)
        {
            if (value != null)
            {
                value = new InsertStringFormatter().Format(value);
            }
            
            SetValue(Placeholders.Reason, value);
        }

        public void SetWorkingOff(string value)
        {
            if (value != null)
            {
                value = new InsertStringFormatter().Format(value, firstLetter: FirstLetter.Upper);
            }
            
            SetValue(Placeholders.WorkingOff, value);
        }
    }
}