using System;
using Lab2.Models.Entities;

namespace Lab2.Models.Analyze;
using System.Xml;
using System.Collections.Generic;

public class SaxAnalyzeStrategy : IAnalyzeStrategy
{
    public List<Student> Search(string filePath, FilterOptions filterOptions)
    {
        var students = new List<Student>();
        Student currentStudent = null;

        using (var reader = XmlReader.Create(filePath))
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "Student")
                    {
                        AddStudentIfMatches(students, currentStudent, filterOptions);
                        currentStudent = ParseStudent(reader);
                    }
                    else if (reader.Name == "DisciplineRecord" && currentStudent != null)
                    {
                        currentStudent.Disciplines.Add(ParseDiscipline(reader));
                    }
                }
            }
            
            AddStudentIfMatches(students, currentStudent, filterOptions);
        }

        return students;
    }
    
    private Student ParseStudent(XmlReader reader)
    {
        return new Student
        {
            StudentName = reader.GetAttribute("StudentName"),
            Faculty = reader.GetAttribute("Faculty"),
            Department = reader.GetAttribute("Department"),
            Disciplines = new List<DisciplineRecord>()
        };
    }
    
    private DisciplineRecord ParseDiscipline(XmlReader reader)
    {
        return new DisciplineRecord
        {
            DisciplineName = reader.GetAttribute("DisciplineName"),
            Grade = int.TryParse(reader.GetAttribute("Grade"), out var g) ? g : 0,
            Credits = int.TryParse(reader.GetAttribute("Credits"), out var c) ? c : 0
        };
    }
    
    private void AddStudentIfMatches(List<Student> students, Student student, FilterOptions filterOptions)
    {
        if (student != null && MatchesFilters(student, filterOptions))
        {
            students.Add(student);
        }
    }

    private bool MatchesFilters(Student student, FilterOptions filterOptions)
    {
        return MatchesProperty(student, filterOptions.Faculty, "Faculty") &&
               MatchesProperty(student, filterOptions.Department, "Department") &&
               MatchesProperty(student, filterOptions.DisciplineName, "DisciplineName") &&
               MatchesKeyword(student, filterOptions.Keyword);
    }

    private bool MatchesProperty(Student student, string filterValue, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(filterValue))
            return true;

        return propertyName switch
        {
            "Faculty" => student.Faculty?.Equals(filterValue, StringComparison.OrdinalIgnoreCase) == true,
            "Department" => student.Department?.Equals(filterValue, StringComparison.OrdinalIgnoreCase) == true,
            "DisciplineName" => HasDiscipline(student, filterValue),
            _ => true
        };
    }
    
    private bool HasDiscipline(Student student, string disciplineName)
    {
        if (student.Disciplines == null)
            return false;
            
        foreach (var d in student.Disciplines)
        {
            if (d.DisciplineName?.Equals(disciplineName, StringComparison.OrdinalIgnoreCase) == true)
                return true;
        }
        
        return false;
    }

    private bool MatchesKeyword(Student student, string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return true;

        if (student.StudentName?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true ||
            student.Faculty?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true ||
            student.Department?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        if (student.Disciplines != null)
        {
            foreach (var d in student.Disciplines)
            {
                if (d.DisciplineName?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true ||
                    d.Grade.ToString().Contains(keyword) ||
                    d.Credits.ToString().Contains(keyword))
                    return true;
            }
        }

        return false;
    }
}