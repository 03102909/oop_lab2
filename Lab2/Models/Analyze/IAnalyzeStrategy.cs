using System.Collections.Generic;
using Lab2.Models.Entities;

namespace Lab2.Models.Analyze;

public interface IAnalyzeStrategy
{
    public List<Student> Search(string filaPath, FilterOptions filterOptions);
}