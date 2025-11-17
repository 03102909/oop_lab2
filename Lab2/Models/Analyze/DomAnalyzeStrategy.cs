using System;
using System.IO;
using System.Xml.Serialization;
using Lab2.Models.Entities;

namespace Lab2.Models.Analyze;
using System.Collections.Generic;
using System.Xml;

public class DomAnalyzeStrategy : IAnalyzeStrategy
{
    public List<Student> Search(string filePath, FilterOptions filterOptions)
    {
        var serializer = new XmlSerializer(typeof(StudentsRoot));
        using var fileStream = new FileStream(filePath, FileMode.Open);
        var root = (StudentsRoot)serializer.Deserialize(fileStream);
        
        var students = root.Students ?? new List<Student>();
        
        return ApplyFilters(students, filterOptions);
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
        if (string.IsNullOrWhiteSpace(filterValue))
            return students;

        var filtered = new List<Student>();

        foreach (var student in students)
        {
            bool matches = propertyName switch
            {
                "Faculty" => student.Faculty?.Equals(filterValue, StringComparison.OrdinalIgnoreCase) == true,
                "Department" => student.Department?.Equals(filterValue, StringComparison.OrdinalIgnoreCase) == true,
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
        if (student.Disciplines == null)
            return false;

        foreach (var d in student.Disciplines)
        {
            if (d.DisciplineName?.Equals(disciplineName, StringComparison.OrdinalIgnoreCase) == true)
                return true;
        }

        return false;
    }
    
    private List<Student> FilterByKeyword(List<Student> students, string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return students;
        
        var filtered = new List<Student>();
        
        foreach (var student in students)
        {
            if (MatchesKeyword(student, keyword))
                filtered.Add(student);
        }
        
        return filtered;
    }
    
    private bool MatchesKeyword(Student student, string keyword)
    {
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