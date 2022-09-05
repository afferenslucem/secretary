using System;
using secretary.documents.utils;
using Xceed.Words.NET;

namespace secretary.documents.templates.docx
{
    public class TimeOffDocumentAdapter: IDisposable
    {
        protected DocX document;
        
        public TimeOffDocumentAdapter(DocX document)
        {
            this.document = document;
        }

        public void SetJobTitle(string value)
        {
            this.document.Paragraphs[2].ReplaceText(Placeholders.JobTitle, value);
        }

        public void SetPersonName(string value)
        {
            this.document.Paragraphs[3].ReplaceText(Placeholders.PersonName, value);
        }

        public void SetTimeOffPeriod(string value)
        {
            value = new InsertStringFormatter().Format(value);
            this.document.Paragraphs[6].ReplaceText(Placeholders.TimeOffPeriod, value);
        }

        public void SetReason(string value)
        {
            if (value == null)
            {
                this.document.Paragraphs[7].RemoveText(0);
                this.document.Paragraphs[7].Hide();
            }
            else
            {
                value = new InsertStringFormatter().Format(value);
                this.document.Paragraphs[7].ReplaceText(Placeholders.Reason, value);
            }
        }

        public void SetWorkingOff(string value)
        {
            if (value == null)
            {
                this.document.Paragraphs[8].RemoveText(0);
                this.document.Paragraphs[8].Hide();
            }
            else
            {
                value = new InsertStringFormatter().Format(value, firstLetter: FirstLetter.Upper);
                this.document.Paragraphs[8].ReplaceText(Placeholders.WorkingOff, value);
            }
        }

        public void SetSendingDay(string value)
        {
            this.document.Paragraphs[10].ReplaceText(Placeholders.SendingDay, value);
        }

        public void SaveAs(string name)
        {
            this.document.SaveAs(name);
        }

        public void Dispose()
        {
            document?.Dispose();
        }
    }
}