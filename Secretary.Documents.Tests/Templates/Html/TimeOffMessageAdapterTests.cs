using HtmlAgilityPack;
using Secretary.Configuration;
using Secretary.Documents.Templates.Html;

namespace Secretary.Documents.Tests.Templates.Html
{
    public class TimeOffMessageAdapterTests
    {
        public HtmlDocument Document = null!;

        [SetUp]
        public void Setup()
        {
            DocumentTemplatesStorage.Initialize(Config.Instance.TemplatesPath);
            Document = DocumentTemplatesStorage.Instance.GetTimeOffMessageTemplate();
        }

        [Test]
        public void ShouldCreateInstance()
        {
            var adapter = new TimeOffMessageAdapter(Document);
            
            Assert.Pass();
        }

        [Test]
        public void ShouldSetJobTitle()
        {
            var adapter = new TimeOffMessageAdapter(Document);
            
            adapter.SetJobTitle("инженер-программист");

            var node = Document.GetElementbyId("signature_job-title");
            
            Assert.That(node.InnerHtml, Is.EqualTo("инженер-программист"));
        }

        [Test]
        public void ShouldSetPersonName()
        {
            var adapter = new TimeOffMessageAdapter(Document);
            
            adapter.SetPersonName("Александр Пушкин");

            var node = Document.GetElementbyId("signature_name");
            
            Assert.That(node.InnerHtml, Is.EqualTo("Александр Пушкин"));
        }

        [Test]
        public void ShouldSetTimeOffDateWithExtraPoint()
        {
            var adapter = new TimeOffMessageAdapter(Document);
            
            adapter.SetTimeOffPeriod("26.12.1825");

            var node = Document.GetElementbyId("time-off_period");
            
            Assert.That(node.InnerHtml, Is.EqualTo("Прошу предоставить отгул 26.12.1825."));
        }

        [Test]
        public void ShouldSetTimeOffDateWithoutExtraPoint()
        {
            var adapter = new TimeOffMessageAdapter(Document);
            
            adapter.SetTimeOffPeriod("26.12.1825.");

            var node = Document.GetElementbyId("time-off_period");
            
            Assert.That(node.InnerHtml, Is.EqualTo("Прошу предоставить отгул 26.12.1825."));
        }

        [Test]
        public void ShouldSetReasonWithExtraPoint()
        {
            var adapter = new TimeOffMessageAdapter(Document);
            
            adapter.SetReason("Нужно съездить на восстание");

            var node = Document.GetElementbyId("time-off_reason");
            
            Assert.That(node.InnerHtml, Is.EqualTo("<br>Причина: Нужно съездить на восстание."));
        }

        [Test]
        public void ShouldSetReasonWithoutExtraPoint()
        {
            var adapter = new TimeOffMessageAdapter(Document);
            
            adapter.SetReason("Нужно съездить на восстание.");

            var node = Document.GetElementbyId("time-off_reason");
            
            Assert.That(node.InnerHtml, Is.EqualTo("<br>Причина: Нужно съездить на восстание."));
        }

        [Test]
        public void ShouldSetReasonWithoutRedundantWhitespaces()
        {
            var adapter = new TimeOffMessageAdapter(Document);
            
            adapter.SetReason("Нужно   съездить на  восстание.");

            var node = Document.GetElementbyId("time-off_reason");
            
            Assert.That(node.InnerHtml, Is.EqualTo("<br>Причина: Нужно съездить на восстание."));
        }

        [Test]
        public void ShouldDeleteReasonIfSetNull()
        {
            var adapter = new TimeOffMessageAdapter(Document);
            
            adapter.SetReason(null);

            var node = Document.GetElementbyId("time-off_reason");
            
            Assert.IsNull(node);
        }

        [Test]
        public void ShouldSetWorkingOffWithExtraPoint()
        {
            var adapter = new TimeOffMessageAdapter(Document);
            
            adapter.SetWorkingOff("Отработаю в ссылке");

            var node = Document.GetElementbyId("time-off_working-off");
            
            Assert.That(node.InnerHtml, Is.EqualTo("<br>Отработаю в ссылке."));
        }

        [Test]
        public void ShouldSetWorkingOffWithoutExtraPoint()
        {
            var adapter = new TimeOffMessageAdapter(Document);
            
            adapter.SetWorkingOff("Отработаю в ссылке.");

            var node = Document.GetElementbyId("time-off_working-off");
            
            Assert.That(node.InnerHtml, Is.EqualTo("<br>Отработаю в ссылке."));
        }

        [Test]
        public void ShouldSetWorkingOffWithUppercase()
        {
            var adapter = new TimeOffMessageAdapter(Document);
            
            adapter.SetWorkingOff("отработаю в ссылке.");

            var node = Document.GetElementbyId("time-off_working-off");
            
            Assert.That(node.InnerHtml, Is.EqualTo("<br>Отработаю в ссылке."));
        }

        [Test]
        public void ShouldDeleteWorkingOffIfSetNull()
        {
            var adapter = new TimeOffMessageAdapter(Document);
            
            adapter.SetWorkingOff(null);

            var node = Document.GetElementbyId("time-off_working-off");
            
            Assert.IsNull(node);
        }

        [Test]
        public void ShouldSave()
        {
            var adapter = new TimeOffMessageAdapter(Document);
            
            var writer = new StringWriter();
            
            adapter.SaveAs(writer);
            
            Assert.That(writer.ToString().Length, Is.GreaterThan(0));
        }
    }
}