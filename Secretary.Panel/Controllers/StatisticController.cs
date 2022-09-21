using Microsoft.AspNetCore.Mvc;
using Secretary.Panel.Models;
using Secretary.Storage;
using Secretary.Storage.Interfaces;

namespace Secretary.Panel.Controllers;

[ApiController]
[Route("[controller]")]
public class StatisticController : ControllerBase
{
    public IEventLogStorage _eventLogStorage = new EventLogStorage();
    public IUserStorage _userStorage = new UserStorage();

    [HttpGet(Name = "GetStatisticData")]
    public async Task<Statistic> GetData()
    {
        var result = new Statistic();

        result.DocumentStatistic = await this.GetDocumentStatistic();
        result.UserStatistic = await this.GetUserStatistic();

        return result;
    }

    [NonAction]
    public async Task<DocumentStatistic> GetDocumentStatistic()
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

    [NonAction]
    public async Task<UserStatistic> GetUserStatistic()
    {
        var result = new UserStatistic()
        {
            TotalUsers = await _userStorage.GetCount(),
            UserWithDocuments = await _userStorage.GetCountWithDocuments(),
        };

        return result;
    }
}