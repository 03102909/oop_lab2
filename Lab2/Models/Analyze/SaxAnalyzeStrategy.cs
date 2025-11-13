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

        using (XmlReader reader = XmlReader.Create(filePath))
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "Student")
                        {
                            if (currentStudent != null && MatchesFilters(currentStudent, filterOptions))
                            {
                                students.Add(currentStudent);
                            }
                            
                            currentStudent = new Student
                            {
                                StudentName = reader.GetAttribute("StudentName") ?? string.Empty,
                                Faculty = reader.GetAttribute("Faculty") ?? string.Empty,
                                Department = reader.GetAttribute("Department") ?? string.Empty,
                                Disciplines = new List<DisciplineRecord>()
                            };
                        }
                        else if (reader.Name == "DisciplineRecord" && currentStudent != null)
                        {
                            var discipline = new DisciplineRecord
                            {
                                DisciplineName = reader.GetAttribute("DisciplineName") ?? string.Empty,
                                Grade = int.TryParse(reader.GetAttribute("Grade"), out int grade) ? grade : 0,
                                Credits = int.TryParse(reader.GetAttribute("Credits"), out int credits) ? credits : 0
                            };
                            currentStudent.Disciplines.Add(discipline);
                        }
                        break;
                }
            }
            
            if (currentStudent != null && MatchesFilters(currentStudent, filterOptions))
            {
                students.Add(currentStudent);
            }
        }

        return students;
    }

    private bool MatchesFilters(Student student, FilterOptions filterOptions)
    {
        if (!MatchesProperty(student, filterOptions.Faculty, "Faculty"))
            return false;
        
        if (!MatchesProperty(student, filterOptions.Department, "Department"))
            return false;
        
        if (!MatchesProperty(student, filterOptions.DisciplineName, "DisciplineName"))
            return false;
        
        if (!MatchesKeyword(student, filterOptions.Keyword))
            return false;

        return true;
    }

    private bool MatchesProperty(Student student, string filterValue, string propertyName)
    {
        if (string.IsNullOrEmpty(filterValue))
            return true;

        switch (propertyName)
        {
            case "Faculty":
                return student.Faculty == filterValue;
            
            case "Department":
                return student.Department == filterValue;
            
            case "DisciplineName":
                foreach (var discipline in student.Disciplines)
                {
                    if (discipline.DisciplineName == filterValue)
                        return true;
                }
                return false;
            
            default:
                return true;
        }
    }

    private bool MatchesKeyword(Student student, string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
            return true;

        if (student.StudentName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            return true;
        
        if (student.Faculty.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            return true;
        
        if (student.Department.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            return true;

        foreach (var discipline in student.Disciplines)
        {
            if (discipline.DisciplineName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return true;
            
            if (discipline.Grade.ToString().Contains(keyword))
                return true;
        }

        return false;
    }
}