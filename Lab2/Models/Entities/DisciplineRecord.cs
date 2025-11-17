using System.Xml.Serialization;

namespace Lab2.Models.Entities;

public class DisciplineRecord
{
    [XmlAttribute("DisciplineName")]
    public string DisciplineName { get; set; }
    
    [XmlAttribute("Grade")]
    public int Grade { get; set; }
    
    [XmlAttribute("Credits")]
    public int Credits { get; set; }
}