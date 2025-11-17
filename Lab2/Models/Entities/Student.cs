using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Lab2.Models.Entities;

[XmlRoot("Students")]
public class StudentsRoot
{
    [XmlElement("Student")]
    public List<Student> Students { get; set; }
}

public class Student
{
    [XmlAttribute("StudentName")]
    public string StudentName { get; set; }
    
    [XmlAttribute("Faculty")]
    public string Faculty { get; set; }
    
    [XmlAttribute("Department")]
    public string Department { get; set; }
    
    [XmlElement("DisciplineRecord")]
    public List<DisciplineRecord> Disciplines { get; set; }
    
    public string DisciplinesFormatted => 
        string.Join("\n", Disciplines.Select(d => 
            $"{d.DisciplineName} (Grade: {d.Grade}, Credits: {d.Credits})"));
}