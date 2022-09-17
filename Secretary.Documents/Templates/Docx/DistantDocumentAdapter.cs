using Secretary.Documents.utils;
using Xceed.Words.NET;

namespace Secretary.Documents.Templates.Docx
{
    public class DistantDocumentAdapter: DocumentAdapter
    {
        public DistantDocumentAdapter(DocX document) : base(document)
        {
        }

        public void SetPeriod(string? value)
        {
            if (value != null)
            {
                value = new InsertStringFormatter().Format(value);
            }
            
            SetValue(Placeholders.Period, value);
        }

        public void SetReason(string? value)
        {
            if (value != null)
            {
                value = new InsertStringFormatter().Format(value);
            }
            
            SetValue(Placeholders.Reason, value);
        }
    }
}