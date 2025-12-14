using SinTachiePlugin.PartAnimation;
using System.Windows;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Control.PartAnimationValueList
{
    /// <summary>
    /// PartAnimationValueListView.xaml の相互作用ロジック
    /// </summary>
    public partial class PartAnimationValueListView : System.Windows.Controls.UserControl, IPropertyEditorControl
    {
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        private List<PartAnimationValue> _clipboad = [];

        public PartAnimationValueListView()
        {
            InitializeComponent();
            DataContextChanged += PointsEditor_DataContextChanged;
        }

        private void PointsEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is PartAnimationValueListViewModel oldVm)
            {
                oldVm.BeginEdit -= PropertiesEditor_BeginEdit;
                oldVm.EndEdit -= PropertiesEditor_EndEdit;
            }
            if (e.NewValue is PartAnimationValueListViewModel newVm)
            {
                newVm.BeginEdit += PropertiesEditor_BeginEdit;
                newVm.EndEdit += PropertiesEditor_EndEdit;
            }
        }

        private void PropertiesEditor_BeginEdit(object? sender, EventArgs e)
        {
            BeginEdit?.Invoke(this, e);
        }

        private void PropertiesEditor_EndEdit(object? sender, EventArgs e)
        {
            //Part内のAnimationを変更した際にPartsを更新する
            //複数のアイテムを選択している場合にすべてのアイテムを更新するために必要
            if (DataContext is PartAnimationValueListViewModel vm)
                vm.CopyToOtherItems();

            EndEdit?.Invoke(this, e);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateButtons();
        }

        public void UpdateButtons()
        {
            if (DataContext is not PartAnimationValueListViewModel vm)
                return;

            List<PartAnimationValue> selecteds = GetSelecteds();

            if (selecteds.Count > 1)
            {
                AddButton.IsEnabled = false;
                RemoveButton.IsEnabled = true;
                UpButton.IsEnabled = false;
                DownButton.IsEnabled = false;
                CutButton.IsEnabled = true;
                CopyButton.IsEnabled = true;
                PasteButton.IsEnabled = false;
                DuplicateButton.IsEnabled = false;
            }
            else
            {
                AddButton.IsEnabled = true;
                PasteButton.IsEnabled = _clipboad.Count > 0;

                if (selecteds.Count < 1)
                {
                    RemoveButton.IsEnabled = false;
                    UpButton.IsEnabled = false;
                    DownButton.IsEnabled = false;
                    CutButton.IsEnabled = false;
                    CopyButton.IsEnabled = false;
                    DuplicateButton.IsEnabled = false;
                }
                else
                {
                    RemoveButton.IsEnabled = true;
                    CutButton.IsEnabled = true;
                    CopyButton.IsEnabled = true;
                    DuplicateButton.IsEnabled = true;

                    if (vm.CanMoveUpItem())
                        UpButton.IsEnabled = true;
                    else
                        UpButton.IsEnabled = false;

                    if (vm.CanMoveDownItem())
                        DownButton.IsEnabled = true;
                    else
                        DownButton.IsEnabled = false;
                }
            }
        }

        private List<PartAnimationValue> GetSelecteds()
        {
            List<PartAnimationValue> selecteds = [];

            foreach (var selected in ItemList.SelectedItems)
                if (selected is PartAnimationValue item)
                    selecteds.Add(item);

            return selecteds;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not PartAnimationValueListViewModel vm)
                return;

            vm.InsertItems([new PartAnimationValue()]);
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not PartAnimationValueListViewModel vm)
                return;

            vm.RemoveItems(GetSelecteds());
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not PartAnimationValueListViewModel vm)
                return;

            vm.MoveUpItem();
            UpdateButtons();
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not PartAnimationValueListViewModel vm)
                return;

            vm.MoveDownItem();
            UpdateButtons();
        }

        private void CutButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not PartAnimationValueListViewModel vm)
                return;

            _clipboad = GetCloneOfSelected();
            vm.RemoveItems(GetSelecteds());
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            _clipboad = GetCloneOfSelected();
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not PartAnimationValueListViewModel vm)
                return;

            vm.InsertItems(GetCloneOfClipboad());
        }

        private void DuplicateButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not PartAnimationValueListViewModel vm)
                return;

            vm.InsertItems(GetCloneOfSelected());
        }

        private List<PartAnimationValue> GetCloneOfSelected()
        {
            return [.. GetSelecteds().Select(v => new PartAnimationValue(v))];
        }

        private List<PartAnimationValue> GetCloneOfClipboad()
        {
            return [.. _clipboad.Select(v => new PartAnimationValue(v))];
        }
    }
}
