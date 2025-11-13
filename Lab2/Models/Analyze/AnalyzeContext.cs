using System.Collections.Generic;
using Lab2.Models.Entities;

namespace Lab2.Models.Analyze;

public class AnalyzeContext
{
    private IAnalyzeStrategy strategy;
    
    public AnalyzeContext() {}
    public AnalyzeContext(IAnalyzeStrategy strategy)
    {
        this.strategy = strategy;
    }

    public void SetStrategy(IAnalyzeStrategy strategy)
    {
        this.strategy = strategy;
    }

    public List<Student> Search(string filePath, FilterOptions filterOptions)
    {
        var result = strategy.Search(filePath, filterOptions);
        return result;
    }
}