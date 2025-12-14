using SinTachiePlugin.Informations;
using SinTachiePlugin.Properties;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TextBox = System.Windows.Controls.TextBox;

namespace SinTachiePlugin.Window_Stp.PartValueTemplateEditor
{
    /// <summary>
    /// AskNameOfNewTemplateDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class AskNameOfNewTemplateDialog : Window
    {
        public string TemplateName { get; private set; } = string.Empty;
        public bool Commited { get; private set; } = false;

        private TextBox? _editableTextBox;
        private string[] _templateNames = [];

        public AskNameOfNewTemplateDialog()
        {
            InitializeComponent();
            CommitButton.IsEnabled = false;
        }

        private void TemplateNameComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            _editableTextBox = (TextBox)TemplateNameComboBox.Template.FindName("PART_EditableTextBox", TemplateNameComboBox);

            if (_editableTextBox != null)
            {
                _editableTextBox.TextChanged += EditableTextBox_TextChanged;
            }
        }

        private void EditableTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CommitButton.IsEnabled = CheckText();

            if (string.IsNullOrEmpty(TemplateNameComboBox.Text))
                PlaceHolder.Visibility = Visibility.Visible;
            else
                PlaceHolder.Visibility = Visibility.Hidden;
        }

        private bool CheckText() => !string.IsNullOrEmpty(TemplateNameComboBox.Text) && !TemplateNameComboBox.Text.EndsWith('/');

        public void SetTemplateNameList(string[] strings)
        {
            TemplateNameComboBox.ItemsSource = strings;
            _templateNames = strings;
        }

        private void TemplateNameComboBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && CheckText())
            {
                CheckSameName();
            }
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            CheckSameName();
        }

        private void CheckSameName()
        {
            if (_templateNames.Contains(TemplateNameComboBox.Text))
                if (SinTachieDialog.GetYESorNO(TextResource.AskNameOfNewTemplateDialog_AskOverwriteTemplate) == System.Windows.Forms.DialogResult.No)
                    return;

            TemplateName = TemplateNameComboBox.Text;
            Commited = true;
            Close();
        }
    }
}
