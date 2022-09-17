namespace Secretary.Documents.Creators.Data;

public class VacationData : DocumentData
{
    public string Name { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public string Period { get; set; } = null!;
}