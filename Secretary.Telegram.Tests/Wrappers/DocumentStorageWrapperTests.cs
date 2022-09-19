using Moq;
using Secretary.Storage;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Wrappers;

namespace Secretary.Telegram.Tests.Wrappers;

public class DocumentStorageWrapperTests
{
    private Mock<IDocumentStorage> _documentStorage = null!;
    private DocumentStorageWrapper _wrapper = null!;
    
    [SetUp]
    public void Setup()
    {
        _documentStorage = new Mock<IDocumentStorage>();
        _wrapper = new DocumentStorageWrapper(_documentStorage.Object, 2517);
    }

    [Test]
    public async Task ShouldGetDocument()
    {
        var expected = new Document();

        _documentStorage.Setup(target => target.GetOrCreateDocument(2517, "/timeoff")).ReturnsAsync(expected);

        var result = await _wrapper.GetOrCreateDocument("/timeoff");
        
        _documentStorage.Verify(target => target.GetOrCreateDocument(2517, "/timeoff"), Times.Once);
        
        Assert.That(result, Is.SameAs(expected));
    }
}