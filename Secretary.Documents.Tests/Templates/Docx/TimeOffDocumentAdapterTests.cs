﻿using Secretary.Configuration;
using Secretary.Documents.Templates.Docx;
using Xceed.Words.NET;

namespace Secretary.Documents.Tests.Templates.Docx;

public class TimeOffDocumentAdapterTests
{
    public DocX Document = null!;

    [SetUp]
    public void Setup()
    {
        DocumentTemplatesStorage.Initialize(Config.Instance.TemplatesPath);
        Document = DocumentTemplatesStorage.Instance.GetTimeOffDocument();
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

        adapter.SetPeriod("10.08.2022 с 9:00 до 13:00");

        Assert.That(Document.Paragraphs[6].Text, Is.EqualTo("Прошу предоставить отгул 10.08.2022 с 9:00 до 13:00."));
    }

    [Test]
    public void ShouldSetPeriodWithoutExtraPoint()
    {
        using var adapter = new TimeOffDocumentAdapter(Document);

        adapter.SetPeriod("10.08.2022 с 9:00 до 13:00.");

        Assert.That(Document.Paragraphs[6].Text, Is.EqualTo("Прошу предоставить отгул 10.08.2022 с 9:00 до 13:00."));
    }

    [Test]
    public void ShouldSetReasonWithExtraPoint()
    {
        using var adapter = new TimeOffDocumentAdapter(Document);

        adapter.SetReason("необходимо починить машину");

        Assert.That(Document.Paragraphs[7].Text, Is.EqualTo("Причина: необходимо починить машину."));
    }

    [Test]
    public void ShouldSetReasonWithoutExtraPoint()
    {
        using var adapter = new TimeOffDocumentAdapter(Document);

        adapter.SetReason("необходимо починить машину.");

        Assert.That(Document.Paragraphs[7].Text, Is.EqualTo("Причина: необходимо починить машину."));
    }

    [Test]
    public void ShouldDeleteReasonIfSetNull()
    {
        using var adapter = new TimeOffDocumentAdapter(Document);

        adapter.SetReason(null);

        Assert.That(Document.Paragraphs[7].Text, Is.EqualTo(""));
    }

    [Test]
    public void ShouldSetWorkingOffWithExtraPoint()
    {
        using var adapter = new TimeOffDocumentAdapter(Document);

        adapter.SetWorkingOff("Пропущенное время обязуюсь отработать");

        Assert.That(Document.Paragraphs[8].Text, Is.EqualTo("Пропущенное время обязуюсь отработать."));
    }

    [Test]
    public void ShouldSetWorkingOffWithoutExtraPoint()
    {
        using var adapter = new TimeOffDocumentAdapter(Document);

        adapter.SetWorkingOff("Пропущенное время обязуюсь отработать.");

        Assert.That(Document.Paragraphs[8].Text, Is.EqualTo("Пропущенное время обязуюсь отработать."));
    }

    [Test]
    public void ShouldSetWorkingOffWithUppercase()
    {
        using var adapter = new TimeOffDocumentAdapter(Document);

        adapter.SetWorkingOff("пропущенное время обязуюсь отработать.");

        Assert.That(Document.Paragraphs[8].Text, Is.EqualTo("Пропущенное время обязуюсь отработать."));
    }

    [Test]
    public void ShouldSetWorkingOffWithoutRedundantWhitespaces()
    {
        using var adapter = new TimeOffDocumentAdapter(Document);

        adapter.SetWorkingOff("Пропущенное  время   обязуюсь  отработать.");

        Assert.That(Document.Paragraphs[8].Text, Is.EqualTo("Пропущенное время обязуюсь отработать."));
    }

    [Test]
    public void ShouldDeleteWorkingOffIfSetNull()
    {
        using var adapter = new TimeOffDocumentAdapter(Document);

        adapter.SetWorkingOff(null);

        Assert.That(Document.Paragraphs[8].Text, Is.EqualTo(""));
    }

    [Test]
    public void ShouldSetSendingDay()
    {
        using var adapter = new TimeOffDocumentAdapter(Document);

        adapter.SetSendingDay("11.08.2022");

        Assert.That(Document.Paragraphs[10].Text, Is.EqualTo("11.08.2022"));
    }
}