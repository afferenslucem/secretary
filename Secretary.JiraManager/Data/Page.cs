namespace Secretary.JiraManager.Data;

public class Page<T>
{
    public int Number { get; init; }
    public int Length { get; init; }
    public int TotalIssues { get; init; }
    public IEnumerable<T> Data { get; init; }
    
    public Page(int number, int length, int totalIssues, IEnumerable<T> data)
    {
        Number = number;
        Length = length;
        TotalIssues = totalIssues;
        Data = data;
    }
}