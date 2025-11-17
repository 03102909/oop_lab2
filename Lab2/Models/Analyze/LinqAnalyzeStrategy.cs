using System.IO;
using System.Xml.Serialization;
using Lab2.Models.Entities;

namespace Lab2.Models.Analyze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

public class LinqAnalyzeStrategy : IAnalyzeStrategy
{
    public List<Student> Search(string filePath, FilterOptions filterOptions) 
    {
        var students = ParseStudentsFromXml(filePath);
        students = ApplyFilters(students, filterOptions);
        return students.ToList();
    }
    
    private IEnumerable<Student> ParseStudentsFromXml(string filePath)
    {
        var serializer = new XmlSerializer(typeof(StudentsRoot));
        
        using var fileStream = new FileStream(filePath, FileMode.Open);
        var root = (StudentsRoot)serializer.Deserialize(fileStream);
        
        return root.Students ?? new List<Student>();
    }
    
    private IEnumerable<Student> FilterByProperty(IEnumerable<Student> students, string filterValue, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(filterValue))
            return students;
    
        return students.Where(s =>
        {
            return propertyName switch
            {
                "Faculty" => s.Faculty?.Equals(filterValue, StringComparison.OrdinalIgnoreCase) == true,
                "Department" => s.Department?.Equals(filterValue, StringComparison.OrdinalIgnoreCase) == true,
                "DisciplineName" => s.Disciplines?.Any(d => 
                    d.DisciplineName?.Equals(filterValue, StringComparison.OrdinalIgnoreCase) == true) == true,
                _ => false
            };
        });
    }
    
    private IEnumerable<Student> FilterByKeyword(IEnumerable<Student> students, string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return students;
            
        return students.Where(s =>
            (s.StudentName?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true) ||
            (s.Faculty?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true) ||
            (s.Department?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true) ||
            (s.Disciplines?.Any(d =>
                (d.DisciplineName?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true) ||
                d.Grade.ToString().Contains(keyword) ||
                d.Credits.ToString().Contains(keyword)
            ) == true)
        );
    }
    
    private IEnumerable<Student> ApplyFilters(IEnumerable<Student> students, FilterOptions filterOptions)
    {
        students = FilterByProperty(students, filterOptions.Faculty, "Faculty");
        students = FilterByProperty(students, filterOptions.Department, "Department");
        students = FilterByProperty(students, filterOptions.DisciplineName, "DisciplineName");
        students = FilterByKeyword(students, filterOptions.Keyword);
    
        return students;
    }
}