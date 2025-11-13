using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Avalonia.Platform.Storage;
using Lab2.Models;
using Lab2.Models.Analyze;
using Lab2.Models.Entities;

namespace Lab2.Views
{
    public partial class MainWindow : Window
    {
        private string xmlFilePath;
        private string xslFilePath;
        private string htmlFilePath;
        
        private AnalyzeContext analyzeContext;
        private List<Student> results;

        public MainWindow()
        {
            InitializeComponent();
            
            analyzeContext = new AnalyzeContext(new LinqAnalyzeStrategy());
            StrategyPicker.SelectionChanged += StrategyPickerSelectionChanged;
    
            ChooseXmlBtn.Click += async (s, e) => await OpenFileBtnClick("xml");
            ChooseXslBtn.Click += async (s, e) => await OpenFileBtnClick("xsl");
            CreateHtmlBtn.Click += SaveHtmlFileBtnClick;
            SearchBtn.Click += SearchBtnClick;
            TransformBtn.Click += OnTransformClick;
        }
        
        private void StrategyPickerSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            IAnalyzeStrategy strategy = StrategyPicker.SelectedIndex switch
            {
                0 => new LinqAnalyzeStrategy(),
                1 => new DomAnalyzeStrategy(),
                2 => new SaxAnalyzeStrategy()
            };
            
            analyzeContext.SetStrategy(strategy);
        }

        private async Task OpenFileBtnClick(string fileType)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = $"Select {fileType.ToUpper()} file",
                AllowMultiple = false,
                FileTypeFilter = new[] { new FilePickerFileType($"{fileType.ToUpper()} Files") { Patterns = new[] { $"*.{fileType}" } } }
            });

            if (files.Count > 0)
            {
                var filePath = files[0].Path.LocalPath;
                StatusText.Text = $"Chosen: {System.IO.Path.GetFileName(filePath)}";
        
                if (fileType == "xml")
                {
                    xmlFilePath = filePath;
                    LoadFilterOptions();
                }
                else if (fileType == "xsl")
                {
                    xslFilePath = filePath;
                }
            }
        }

        private async void SaveHtmlFileBtnClick(object sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save HTML File",
                FileTypeChoices = new[] { new FilePickerFileType("HTML Files") { Patterns = new[] { "*.html" } } }
            });

            if (file != null)
            {
                htmlFilePath = file.Path.LocalPath;
                StatusText.Text = $"HTML will be saved to: {System.IO.Path.GetFileName(htmlFilePath)}";
            }
        }

        private void LoadFilterOptions()
        {
            try
            {
                var doc = XDocument.Load(xmlFilePath);
                
                PopulateComboBox(FacultyBox, 
                    doc.Descendants("Student").Select(s => s.Attribute("Faculty")?.Value));
                
                PopulateComboBox(DepartmentBox, 
                    doc.Descendants("Student").Select(s => s.Attribute("Department")?.Value));
                
                PopulateComboBox(DisciplineNameBox, 
                    doc.Descendants("DisciplineRecord").Select(d => d.Attribute("DisciplineName")?.Value));
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error loading filter options: {ex.Message}";
            }
        }
        
        private void PopulateComboBox(ComboBox comboBox, IEnumerable<string> values)
        {
            comboBox.Items.Clear();
            comboBox.Items.Add("All");
    
            values
                .Where(v => !string.IsNullOrEmpty(v))
                .Distinct()
                .ToList()
                .ForEach(v => comboBox.Items.Add(v));
    
            comboBox.SelectedIndex = 0;
        }

        private void SearchBtnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(xmlFilePath))
            {
                StatusText.Text = "Choose XML file first";
                return;
            }

            try
            {
                var filterOptions = new FilterOptions
                {
                    Faculty = FacultyBox.SelectedItem?.ToString() == "All" ? null : FacultyBox.SelectedItem?.ToString(),
                    Department = DepartmentBox.SelectedItem?.ToString() == "All" ? null : DepartmentBox.SelectedItem?.ToString(),
                    DisciplineName = DisciplineNameBox.SelectedItem?.ToString() == "All" ? null : DisciplineNameBox.SelectedItem?.ToString(),
                    Keyword = string.IsNullOrWhiteSpace(KeywordBox.Text) ? null : KeywordBox.Text
                };

                results = analyzeContext.Search(xmlFilePath, filterOptions);
                
                OutputGrid.ItemsSource = results;
                StatusText.Text = $"Found records: {results.Count}";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
            }
        }

        private void OnTransformClick(object sender, RoutedEventArgs e)
        {
            var transformer = new Transform(); 
            transformer.TransformToHtml(results, xslFilePath, htmlFilePath);
        }
    }
}