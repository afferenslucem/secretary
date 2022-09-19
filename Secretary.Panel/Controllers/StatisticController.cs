using Microsoft.AspNetCore.Mvc;
using Secretary.Panel.Models;
using Secretary.Storage;
using Secretary.Storage.Interfaces;

namespace Secretary.Panel.Controllers;

[ApiController]
[Route("[controller]")]
public class StatisticController : ControllerBase
{
    private readonly IEventLogStorage _eventLogStorage = new EventLogStorage();
    private readonly IUserStorage _userStorage = new UserStorage();

    [HttpGet(Name = "GetStatistic")]
    public async Task<Statistic> GetData()
    {
        var result = new Statistic();

        result.DocumentStatistic = await this.GetDocumentStatistic();
        result.UserStatistic = await this.GetUserStatistic();

        return result;
    }

    private async Task<DocumentStatistic> GetDocumentStatistic()
    {
        var data = await _eventLogStorage.GetDocumentStatistic("/timeoff", "/distant", "/vacation");

        var result = new DocumentStatistic
        {
            DistantCount = data.FirstOrDefault(
                item => item.DocumentName == "/distant", 
                (DocumentName: "", Count: 0)
            ).Count,
            TimeOffCount = data.FirstOrDefault(
                item => item.DocumentName == "/timeoff",
                (DocumentName: "", Count: 0)
            ).Count,
            VacationCount = data.FirstOrDefault(
                item => item.DocumentName == "/vacation", 
                (DocumentName: "", Count: 0)
            ).Count,
        };

        return result;
    }

    private async Task<UserStatistic> GetUserStatistic()
    {
        var result = new UserStatistic()
        {
            TotalUsers = await _userStorage.GetCount(),
            UserWithDocuments = await _userStorage.GetCountWithDocuments(),
        };

        return result;
    }
}