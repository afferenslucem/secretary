using Secretary.Configuration;
using Secretary.Documents.Templates.Docx;
using Xceed.Words.NET;

namespace Secretary.Documents.Tests.Templates.Docx;

public class DistantDocumentAdapterTests
{
    public DocX Document = null!;

    [SetUp]
    public void Setup()
    {
        DocumentTemplatesStorage.Initialize(Config.Instance.TemplatesPath);
        Document = DocumentTemplatesStorage.Instance.GetDistantDocument();
    }

    [Test]
    public void ShouldCreateInstance()
    {
        using var adapter = new TimeOffDocumentAdapter(Document);

        Assert.Pass();
    }

    [Test]
    public void ShouldSetJobTitle()
    {
        using var adapter = new TimeOffDocumentAdapter(Document);

        adapter.SetJobTitle("инженера-программиста");

        Assert.That(Document.Paragraphs[2].Text, Is.EqualTo("от инженера-программиста"));
    }

    [Test]
    public void ShouldSetPersonName()
    {
        using var adapter = new TimeOffDocumentAdapter(Document);

        adapter.SetPersonName("Иванова Ивана Ивановича");

        Assert.That(Document.Paragraphs[3].Text, Is.EqualTo("Иванова Ивана Ивановича"));
    }

    [Test]
    public void ShouldSetPeriodWithExtraPoint()
    {
        using var adapter = new TimeOffDocumentAdapter(Document);

        adapter.SetPeriod("c 10.08.2022 на неопределенный срок");

        Assert.That(Document.Paragraphs[6].Text, Is.EqualTo("Прошу разрешить удаленную работу c 10.08.2022 на неопределенный срок."));
    }

    [Test]
    public void ShouldSetPeriodWithoutExtraPoint()
    {
        using var adapter = new TimeOffDocumentAdapter(Document);

        adapter.SetPeriod("c 10.08.2022 на неопределенный срок.");

        Assert.That(Document.Paragraphs[6].Text, Is.EqualTo("Прошу разрешить удаленную работу c 10.08.2022 на неопределенный срок."));
    }

    [Test]
    public void ShouldSetReasonWithExtraPoint()
    {
        using var adapter = new TimeOffDocumentAdapter(Document);

        adapter.SetReason("плохое самочувствие");

        Assert.That(Document.Paragraphs[7].Text, Is.EqualTo("Причина: плохое самочувствие."));
    }

    [Test]
    public void ShouldSetReasonWithoutExtraPoint()
    {
        using var adapter = new TimeOffDocumentAdapter(Document);

        adapter.SetReason("плохое самочувствие.");

        Assert.That(Document.Paragraphs[7].Text, Is.EqualTo("Причина: плохое самочувствие."));
    }

    [Test]
    public void ShouldDeleteReasonIfSetNull()
    {
        using var adapter = new TimeOffDocumentAdapter(Document);

        adapter.SetReason(null);

        Assert.That(Document.Paragraphs[7].Text, Is.EqualTo(""));
    }

    [Test]
    public void ShouldSetSendingDay()
    {
        using var adapter = new TimeOffDocumentAdapter(Document);

        adapter.SetSendingDay("11.08.2022");

        Assert.That(Document.Paragraphs[10].Text, Is.EqualTo("11.08.2022"));
    }
}