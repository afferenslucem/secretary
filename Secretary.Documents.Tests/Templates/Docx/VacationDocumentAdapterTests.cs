using Secretary.Configuration;
using Secretary.Documents.Templates.Docx;
using Xceed.Words.NET;

namespace Secretary.Documents.Tests.Templates.Docx;

public class VacationDocumentAdapterTests
{
    public DocX Document = null!;

    [SetUp]
    public void Setup()
    {
        DocumentTemplatesStorage.Initialize(Config.Instance.TemplatesPath);
        Document = DocumentTemplatesStorage.Instance.GetVacationDocument();
    }

    [Test]
    public void ShouldCreateInstance()
    {
        using var adapter = new VacationDocumentAdapter(Document);

        Assert.Pass();
    }

    [Test]
    public void ShouldSetJobTitle()
    {
        using var adapter = new VacationDocumentAdapter(Document);

        adapter.SetJobTitle("инженера-программиста");

        Assert.That(Document.Paragraphs[2].Text, Is.EqualTo("от инженера-программиста"));
    }

    [Test]
    public void ShouldSetPersonName()
    {
        using var adapter = new VacationDocumentAdapter(Document);

        adapter.SetPersonName("Иванова Ивана Ивановича");

        Assert.That(Document.Paragraphs[3].Text, Is.EqualTo("Иванова Ивана Ивановича"));
    }

    [Test]
    public void ShouldSetPeriodWithoutExtraPoint()
    {
        using var adapter = new VacationDocumentAdapter(Document);

        adapter.SetPeriod("с 10.08.2022 по 23.08.2022");

        Assert.That(Document.Paragraphs[6].Text, Is.EqualTo("Прошу предоставить ежегодный оплачиваемый отпуск с 10.08.2022 по 23.08.2022, включительно."));
    }

    [Test]
    public void ShouldSetSendingDay()
    {
        using var adapter = new VacationDocumentAdapter(Document);

        adapter.SetSendingDay("11.08.2022");

        Assert.That(Document.Paragraphs[8].Text, Is.EqualTo("11.08.2022"));
    }
}