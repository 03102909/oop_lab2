using System.Collections.Generic;
using System.Linq;

namespace Lab2.Models.Entities;

public class Student
{
    public string StudentName { get; set; }
    public string Faculty { get; set; }
    public string Department { get; set; }
    public List<DisciplineRecord> Disciplines { get; set; }
    
    public string DisciplinesFormatted => 
        string.Join("\n", Disciplines.Select(d => $"{d.DisciplineName} (Grade: {d.Grade}, Credits: {d.Credits})"));
}