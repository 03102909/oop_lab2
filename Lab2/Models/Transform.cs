using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Lab2.Models.Entities;

namespace Lab2.Models;

public class Transform
{
    public void TransformToHtml(List<Student> filtered, string xslPath, string outputPath)
    {
        var xmlDoc = new XDocument(
            new XElement("Students",
                filtered.Select(student =>
                    new XElement("Student",
                        new XAttribute("StudentName", student.StudentName ?? ""),
                        new XAttribute("Faculty", student.Faculty ?? ""),
                        new XAttribute("Department", student.Department ?? ""),
                        student.Disciplines.Select(discipline =>
                            new XElement("DisciplineRecord",
                                new XAttribute("DisciplineName", discipline.DisciplineName ?? ""),
                                new XAttribute("Grade", discipline.Grade),
                                new XAttribute("Credits", discipline.Credits)
                            )
                        )
                    )
                )
            )
        );
        
        var xslt = new XslCompiledTransform();
        xslt.Load(xslPath);
        
        using (var writer = new StreamWriter(outputPath))
        {
            xslt.Transform(xmlDoc.CreateReader(), null, writer);
        }
    }
}