using System.Xml.Serialization;

namespace Secretary.WorkingCalendar.Models;


[XmlType(TypeName = "holiday")]
public class Holiday
{
    [XmlAttribute("id")] public int Id { get; set; }

    [XmlAttribute("title")] public string Title { get; set; } = null!;

    public override string ToString()
    {
        return Title;
    }
}