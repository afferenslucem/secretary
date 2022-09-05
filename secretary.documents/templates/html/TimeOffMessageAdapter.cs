using System.IO;
using HtmlAgilityPack;
using secretary.documents.utils;

namespace secretary.documents.templates.html
{
    public class TimeOffMessageAdapter
    {
        protected HtmlDocument Doc;
        
        public TimeOffMessageAdapter(HtmlDocument doc)
        {
            this.Doc = doc;
        }

        public void SetJobTitle(string value)
        {
            var jobTitle = Doc.GetElementbyId("signature_job-title");

            var signature = jobTitle.ParentNode;
                
            var replaced = jobTitle.OuterHtml.Replace(Placeholders.JobTitle, value);

            var newNode = HtmlNode.CreateNode(replaced);

            signature.ReplaceChild(newNode, jobTitle);
        }

        public void SetPersonName(string value)
        {
            
            var name = Doc.GetElementbyId("signature_name");

            var signature = name.ParentNode;
                
            var replaced = name.OuterHtml.Replace(Placeholders.PersonName, value);

            var newNode = HtmlNode.CreateNode(replaced);

            signature.ReplaceChild(newNode, name);
        }

        public void SetTimeOffPeriod(string value)
        {
            var period = Doc.GetElementbyId("time-off_period");

            var body = period.ParentNode;

            value = new InsertStringFormatter().Format(value);
                
            var replaced = period.OuterHtml.Replace(Placeholders.TimeOffPeriod, value);

            var newNode = HtmlNode.CreateNode(replaced);

            body.ReplaceChild(newNode, period);
        }

        public void SetReason(string value)
        {
            var reason = Doc.GetElementbyId("time-off_reason");

            var parent = reason.ParentNode;
             
            if (value == null)
            {
                parent.RemoveChild(reason);
            }
            else
            {
                value = new InsertStringFormatter().Format(value);
                
                var replaced = reason.OuterHtml.Replace(Placeholders.Reason, value);

                var newNode = HtmlNode.CreateNode(replaced);

                parent.ReplaceChild(newNode, reason);
            }
        }

        public void SetWorkingOff(string value)
        {
            var reason = Doc.GetElementbyId("time-off_working-off");

            var parent = reason.ParentNode;
             
            if (value == null)
            {
                parent.RemoveChild(reason);
            }
            else
            {
                value = new InsertStringFormatter().Format(value, firstLetter: FirstLetter.Upper);
                
                var replaced = reason.OuterHtml.Replace(Placeholders.WorkingOff, value);

                var newNode = HtmlNode.CreateNode(replaced);

                parent.ReplaceChild(newNode, reason);
            }
        }

        public void SaveAs(TextWriter writer)
        {
            this.Doc.Save(writer);
        }
    }
}