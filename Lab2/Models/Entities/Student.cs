using System.Collections.Generic;

namespace Lab2.Models.Entities;

public class Student
{
    public string StudentName { get; set; }
    public string Faculty { get; set; }
    public string Department { get; set; }
    public List<DisciplineRecord> Disciplines { get; set; }
}