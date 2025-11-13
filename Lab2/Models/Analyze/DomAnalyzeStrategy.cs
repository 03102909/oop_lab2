using Lab2.Models.Entities;

namespace Lab2.Models.Analyze;
using System.Collections.Generic;
using System.Xml;

public class DomAnalyzeStrategy : IAnalyzeStrategy
{
    public List<Student> Search(string filePath, FilterOptions filterOptions)
    {
        var doc = new XmlDocument();
        doc.Load(filePath);
        
        var students = ParseStudentsFromXml(doc);
        
        students = ApplyFilters(students, filterOptions);
        
        return students;
    }
    
    private List<Student> ParseStudentsFromXml(XmlDocument doc)
    {
        var studentNodes = doc.SelectNodes("//Student");
        var students = new List<Student>();
        
        foreach (XmlNode studentNode in studentNodes)
        {
            var student = new Student
            {
                StudentName = studentNode.Attributes?["StudentName"]?.Value,
                Faculty = studentNode.Attributes?["Faculty"]?.Value,
                Department = studentNode.Attributes?["Department"]?.Value,
                Disciplines = new List<DisciplineRecord>()
            };
            
            var disciplineNodes = studentNode.SelectNodes("DisciplineRecord");
            foreach (XmlNode disciplineNode in disciplineNodes)
            {
                var discipline = new DisciplineRecord
                {
                    DisciplineName = disciplineNode.Attributes?["DisciplineName"]?.Value,
                    Grade = int.Parse(disciplineNode.Attributes?["Grade"]?.Value ?? "0"),
                    Credits = int.Parse(disciplineNode.Attributes?["Credits"]?.Value ?? "0")
                };
                student.Disciplines.Add(discipline);
            }
            
            students.Add(student);
        }
        
        return students;
    }
    
    private List<Student> ApplyFilters(List<Student> students, FilterOptions filterOptions)
    {
        students = FilterByProperty(students, filterOptions.Faculty, "Faculty");
        students = FilterByProperty(students, filterOptions.Department, "Department");
        students = FilterByProperty(students, filterOptions.DisciplineName, "DisciplineName");
        students = FilterByKeyword(students, filterOptions.Keyword);
        
        return students;
    }
    
    private List<Student> FilterByProperty(List<Student> students, string filterValue, string propertyName)
    {
        if (string.IsNullOrEmpty(filterValue))
            return students;
        
        var filtered = new List<Student>();
        
        foreach (var student in students)
        {
            bool matches = propertyName switch
            {
                "Faculty" => student.Faculty == filterValue,
                "Department" => student.Department == filterValue,
                "DisciplineName" => HasDiscipline(student, filterValue),
                _ => false
            };
            
            if (matches)
                filtered.Add(student);
        }
        
        return filtered;
    }
    
    private bool HasDiscipline(Student student, string disciplineName)
    {
        foreach (var discipline in student.Disciplines)
        {
            if (discipline.DisciplineName == disciplineName)
                return true;
        }
        return false;
    }
    
    private List<Student> FilterByKeyword(List<Student> students, string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
            return students;
        
        var filtered = new List<Student>();
        keyword = keyword.ToLower();
        
        foreach (var student in students)
        {
            if (MatchesKeyword(student, keyword))
                filtered.Add(student);
        }
        
        return filtered;
    }
    
    private bool MatchesKeyword(Student student, string keyword)
    {
        if (student.StudentName?.ToLower().Contains(keyword) == true)
            return true;
        
        if (student.Faculty?.ToLower().Contains(keyword) == true)
            return true;
        
        if (student.Department?.ToLower().Contains(keyword) == true)
            return true;
        
        foreach (var discipline in student.Disciplines)
        {
            if (discipline.DisciplineName?.ToLower().Contains(keyword) == true)
                return true;
            
            if (discipline.Grade.ToString().Contains(keyword))
                return true;
        }
        
        return false;
    }
}