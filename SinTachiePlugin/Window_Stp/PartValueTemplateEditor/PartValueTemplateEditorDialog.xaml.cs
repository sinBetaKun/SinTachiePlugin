using SinTachiePlugin.Informations;
using SinTachiePlugin.Part;
using SinTachiePlugin.Parts;
using SinTachiePlugin.Properties;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SinTachiePlugin.Window_Stp.PartValueTemplateEditor
{
    /// <summary>
    /// PartValueTemplateEditorDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class PartValueTemplateEditorDialog : Window
    {
        private string _sourcePath = string.Empty;
        private PartSourceInfo? _partSourceInfo;
        private List<ControlledParametersOfPartTemplate> _templateListSource = [];

        internal ControlledParametersOfPartTemplate? SelectedTemplate;
        internal PartValue? PartValue;

        public PartValueTemplateEditorDialog()
        {
            InitializeComponent();
            SelectedTemplate = null;
            ReadButton.IsEnabled = false;
            OverwriteButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            TemplateList.SelectedIndex = -1;
        }

        public void SetSourcePath(string sourcePath)
        {
            _sourcePath = sourcePath;

            if (PartInfo.ReadStpi(sourcePath) is PartInfo partInfo)
            {
                _partSourceInfo = new(partInfo);
                _partSourceInfo.Export();
                File.Delete(partInfo.StpiPath);
            }
            else if (PartSourceInfo.ReadStpsi(sourcePath) is PartSourceInfo partSourceInfo)
            {
                _partSourceInfo = partSourceInfo;
            }

            UpdateTemplateList();
        }

        private void UpdateTemplateList()
        {
            if (_partSourceInfo is null)
                return;

            _templateListSource = [.. _partSourceInfo.Templates];
            _templateListSource.Sort((a, b) => a.Name.CompareTo(b.Name));
            TemplateList.Items.Clear();

            foreach (var template in _templateListSource)
            {
                TextBlock textBlock = new()
                {
                    Text = template.Name,
                };
                StackPanel stackPanel = new()
                {
                    Orientation = System.Windows.Controls.Orientation.Horizontal,
                };
                stackPanel.Children.Add(textBlock);
                TemplateList.Items.Add(stackPanel);
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            if (PartValue is null || _partSourceInfo is null)
                return;

            string[] templateNames = [.. _templateListSource.Select(t => t.Name)];
            AskNameOfNewTemplateDialog dialog = new();
            dialog.SetTemplateNameList(templateNames);
            dialog.ShowDialog();

            if (dialog.Commited)
            {
                if (_templateListSource.FirstOrDefault(t => t.Name == dialog.TemplateName) is ControlledParametersOfPartTemplate template)
                {
                    template.Param = new(PartValue.ControlledParameters);
                }
                else
                {
                    _partSourceInfo.Templates.Add(new(dialog.TemplateName, PartValue.ControlledParameters));
                    UpdateTemplateList();
                    _partSourceInfo.Templates = _templateListSource;
                }

                _partSourceInfo.Export();
            }
        }

        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedTemplate = _templateListSource[TemplateList.SelectedIndex];
            Close();
        }

        private void OverwriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_partSourceInfo is null || PartValue is null)
                return;

            string message =
                TextResource.PartValueTemplateEditorDialog_Warning_OverWriteTemplate
                .Replace("{0}", _sourcePath)
                .Replace("{1}", _templateListSource[TemplateList.SelectedIndex].Name);
            DialogResult result = SinTachieDialog.GetYESorNO(message);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                _templateListSource[TemplateList.SelectedIndex].Param = new(PartValue.ControlledParameters);
                _partSourceInfo.Export();
            }
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            if (PartValue is null || _partSourceInfo is null)
                return;

            string[] templateNames = [.. _templateListSource.Select(t => t.Name)];
            AskNameOfNewTemplateDialog dialog = new();
            dialog.SetTemplateNameList(templateNames);
            dialog.ShowDialog();

            if (dialog.Commited)
            {
                if (_templateListSource.FirstOrDefault(t => t.Name == dialog.TemplateName) is ControlledParametersOfPartTemplate template)
                {
                    if (_templateListSource[TemplateList.SelectedIndex] == template)
                        return;

                    _partSourceInfo.Templates.Remove(template);
                }

                _templateListSource[TemplateList.SelectedIndex].Name = dialog.TemplateName;
                UpdateTemplateList();
                _partSourceInfo.Templates = _templateListSource;
                _partSourceInfo.Export();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_partSourceInfo is null)
                return;

            string message =
                TextResource.PartValueTemplateEditorDialog_Warning_DeleteTemplate
                .Replace("{0}", _sourcePath)
                .Replace("{1}", _templateListSource[TemplateList.SelectedIndex].Name);
            DialogResult result = SinTachieDialog.GetYESorNO(message);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                _partSourceInfo.Templates.Remove(_templateListSource[TemplateList.SelectedIndex]);
                UpdateTemplateList();
                _partSourceInfo.Export();
            }
        }

        private void TemplateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TemplateList.SelectedIndex > -1 && TemplateList.SelectedIndex < _templateListSource.Count)
            {
                ReadButton.IsEnabled = true;
                OverwriteButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
            }
            else
            {
                ReadButton.IsEnabled = false;
                OverwriteButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
            }
        }
    }
}
