using Moq;
using Secretary.Panel.Controllers;
using Secretary.Storage.Interfaces;

namespace Secretary.Panel.Tests;

public class StatisticControllerTests
{
    private StatisticController _controller = null!;
    
    private Mock<IUserStorage> _userStorage = null!;
    
    private Mock<IEventLogStorage> _eventLogStorage = null!;

    [SetUp]
    public void Setup()
    {
        _userStorage = new Mock<IUserStorage>();
        _eventLogStorage = new Mock<IEventLogStorage>();
        _controller = new StatisticController();

        _controller._userStorage = _userStorage.Object;
        _controller._eventLogStorage = _eventLogStorage.Object;
    }

    [Test]
    public async Task ShouldReturnZerosForEmptyData()
    {
        _eventLogStorage
            .Setup(target => target.GetDocumentStatistic(It.IsAny<string[]>()))
            .ReturnsAsync(Array.Empty<(string, int)>());

        _userStorage
            .Setup(target => target.GetCount())
            .ReturnsAsync(0);

        _userStorage
            .Setup(target => target.GetCountWithDocuments())
            .ReturnsAsync(0);

        var result = await _controller.GetData();
        
        Assert.That(result.DocumentStatistic!.TimeOffCount, Is.EqualTo(0));
        Assert.That(result.DocumentStatistic!.VacationCount, Is.EqualTo(0));
        Assert.That(result.DocumentStatistic!.DistantCount, Is.EqualTo(0));
        Assert.That(result.UserStatistic!.TotalUsers, Is.EqualTo(0));
        Assert.That(result.UserStatistic!.UserWithDocuments, Is.EqualTo(0));
    }

    [Test]
    public async Task ShouldConvertDocumentData()
    {
        _eventLogStorage
            .Setup(target => target.GetDocumentStatistic(It.IsAny<string[]>()))
            .ReturnsAsync(
                new []
                {
                    ("/timeoff", 1),
                    ("/vacation", 2),
                    ("/distant", 3),
                });

        _userStorage
            .Setup(target => target.GetCountWithDocuments())
            .ReturnsAsync(4);

        _userStorage
            .Setup(target => target.GetCount())
            .ReturnsAsync(5);

        var result = await _controller.GetData();
        
        Assert.That(result.DocumentStatistic!.TimeOffCount, Is.EqualTo(1));
        Assert.That(result.DocumentStatistic!.VacationCount, Is.EqualTo(2));
        Assert.That(result.DocumentStatistic!.DistantCount, Is.EqualTo(3));
        Assert.That(result.UserStatistic!.UserWithDocuments, Is.EqualTo(4));
        Assert.That(result.UserStatistic!.TotalUsers, Is.EqualTo(5));
    }
}