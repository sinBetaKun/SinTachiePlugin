using System.Windows;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Control.PartAnimationValueList
{
    /// <summary>
    /// PartAnimationValueViewer.xaml の相互作用ロジック
    /// </summary>
    public partial class PartAnimationValueViewer : System.Windows.Controls.UserControl, IPropertyEditorControl
    {
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public PartAnimationValueViewer()
        {
            InitializeComponent();
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
                nameof(IsSelected),
                typeof(bool),
                typeof(PartAnimationValueViewer),
                new PropertyMetadata(false, OnStatusChanged));

        private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PartAnimationValueViewer control)
            {
                control.UpdateUIStyle();
            }
        }

        private void UpdateUIStyle()
        {
            if (IsSelected)
                SelectedSign.Visibility = Visibility.Visible;
            else
                SelectedSign.Visibility = Visibility.Hidden;
        }
    }
}
