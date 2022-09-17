using HtmlAgilityPack;
using Secretary.Configuration;
using Secretary.Documents.Templates.Html;

namespace Secretary.Documents.Tests.Templates.Html
{
    public class DistantMessageAdapterTests
    {
        public HtmlDocument Document = null!;

        [SetUp]
        public void Setup()
        {
            DocumentTemplatesStorage.Initialize(Config.Instance.TemplatesPath);
            Document = DocumentTemplatesStorage.Instance.GetDistantMessageTemplate();
        }

        [Test]
        public void ShouldCreateInstance()
        {
            var adapter = new DistantMessageAdapter(Document);
            
            Assert.Pass();
        }

        [Test]
        public void ShouldSetJobTitle()
        {
            var adapter = new DistantMessageAdapter(Document);
            
            adapter.SetJobTitle("инженер-программист");

            var node = Document.GetElementbyId("signature_job-title");
            
            Assert.That(node.InnerHtml, Is.EqualTo("инженер-программист"));
        }

        [Test]
        public void ShouldSetPersonName()
        {
            var adapter = new DistantMessageAdapter(Document);
            
            adapter.SetPersonName("Александр Пушкин");

            var node = Document.GetElementbyId("signature_name");
            
            Assert.That(node.InnerHtml, Is.EqualTo("Александр Пушкин"));
        }

        [Test]
        public void ShouldSetTimeOffDateWithExtraPoint()
        {
            var adapter = new DistantMessageAdapter(Document);
            
            adapter.SetTimeOffPeriod("08.02.1837");

            var node = Document.GetElementbyId("distant_period");
            
            Assert.That(node.InnerHtml, Is.EqualTo("Прошу разрешить удаленную работу 08.02.1837."));
        }

        [Test]
        public void ShouldSetTimeOffDateWithoutExtraPoint()
        {
            var adapter = new DistantMessageAdapter(Document);
            
            adapter.SetTimeOffPeriod("08.02.1837.");

            var node = Document.GetElementbyId("distant_period");
            
            Assert.That(node.InnerHtml, Is.EqualTo("Прошу разрешить удаленную работу 08.02.1837."));
        }

        [Test]
        public void ShouldSetReasonWithExtraPoint()
        {
            var adapter = new DistantMessageAdapter(Document);
            
            adapter.SetReason("пулевое ранение");

            var node = Document.GetElementbyId("distant_reason");
            
            Assert.That(node.InnerHtml, Is.EqualTo("<br>Причина: пулевое ранение."));
        }

        [Test]
        public void ShouldSetReasonWithoutExtraPoint()
        {
            var adapter = new DistantMessageAdapter(Document);
            
            adapter.SetReason("пулевое ранение.");

            var node = Document.GetElementbyId("distant_reason");
            
            Assert.That(node.InnerHtml, Is.EqualTo("<br>Причина: пулевое ранение."));
        }

        [Test]
        public void ShouldSetReasonWithoutRedundantWhitespaces()
        {
            var adapter = new DistantMessageAdapter(Document);
            
            adapter.SetReason("пулевое     ранение.");

            var node = Document.GetElementbyId("distant_reason");
            
            Assert.That(node.InnerHtml, Is.EqualTo("<br>Причина: пулевое ранение."));
        }

        [Test]
        public void ShouldDeleteReasonIfSetNull()
        {
            var adapter = new DistantMessageAdapter(Document);
            
            adapter.SetReason(null);

            var node = Document.GetElementbyId("distant_reason");
            
            Assert.IsNull(node);
        }

        [Test]
        public void ShouldSave()
        {
            var adapter = new DistantMessageAdapter(Document);

            var writer = new StringWriter();
            
            adapter.SaveAs(writer);
            
            Assert.That(writer.ToString().Length, Is.GreaterThan(0));
        }
    }
}