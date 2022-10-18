using Secretary.Documents.utils;
using Xceed.Words.NET;

namespace Secretary.Documents.Templates.Docx
{
    public class VacationDocumentAdapter: DocumentAdapter
    {
        public VacationDocumentAdapter(DocX document) : base(document)
        {
        }

        public void SetPeriod(string? value)
        {
            if (value != null)
            {
                value = new InsertStringFormatter().Format(value, extraPoint: false);
            }
            
            SetValue(Placeholders.Period, value);
        }
    }
}