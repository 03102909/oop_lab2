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
        var xmlDoc = XDocument.Load(filePath);
        
        var students = ParseStudentsFromXml(xmlDoc);
        
        students = ApplyFilters(students, filterOptions);
        
        return students.ToList();
    }
    
    private IEnumerable<Student> ParseStudentsFromXml(XDocument xmlDoc)
    {
        return xmlDoc.Descendants("Student")
            .Select(s => new Student
            {
                StudentName = (string)s.Attribute("StudentName"),
                Faculty = (string)s.Attribute("Faculty"),
                Department = (string)s.Attribute("Department"),
                Disciplines = s.Elements("DisciplineRecord")
                    .Select(d => new DisciplineRecord
                    {
                        DisciplineName = (string)d.Attribute("DisciplineName"),
                        Grade = (int)d.Attribute("Grade"),
                        Credits = (int)d.Attribute("Credits")
                    }).ToList()
            });
    }
    
    private IEnumerable<Student> FilterByProperty(IEnumerable<Student> students, string filterValue, string propertyName)
    {
        if (string.IsNullOrEmpty(filterValue))
            return students;
    
        return students.Where(s =>
        {
            return propertyName switch
            {
                "Faculty" => s.Faculty == filterValue,
                "Department" => s.Department == filterValue,
                "DisciplineName" => s.Disciplines.Any(d => d.DisciplineName == filterValue),
                _ => false
            };
        });
    }
    
    private IEnumerable<Student> FilterByKeyword(IEnumerable<Student> students, string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
            return students;
            
        return students.Where(s =>
            s.StudentName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            s.Faculty.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            s.Department.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            s.Disciplines.Any(d =>
                d.DisciplineName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                d.Grade.ToString().Contains(keyword)
            )
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