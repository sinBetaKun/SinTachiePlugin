using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using SinTachiePlugin.Part;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter;
using SinTachiePlugin.Parts;
using SinTachiePlugin.Properties;
using SinTachiePlugin.Window_Stp.PartValueTemplateEditor;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Settings;

namespace SinTachiePlugin.Control.PartValueList
{
    /// <summary>
    /// PartValueListView.xaml の相互作用ロジック
    /// </summary>
    public partial class PartValueListView : System.Windows.Controls.UserControl, IPropertyEditorControl2
    {
        private class NaturalStringComparer : IComparer<TreeViewItem>
        {
            [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
            static extern int StrCmpLogicalW(string x, string y);

            public int Compare(TreeViewItem x, TreeViewItem y) => StrCmpLogicalW((string)x.Header, (string)y.Header);
        }

        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        private List<PartValue> _clipboad = [];

        private LayerType _addingPartType = LayerType.Image;

        private string _lastAddedPartPath = string.Empty;
        private readonly List<(TreeViewItem, string)> _nameTreeItems = [];

        public PartValueListView()
        {
            InitializeComponent();
            DataContextChanged += PointsEditor_DataContextChanged;
        }
        public void SetEditorInfo(IEditorInfo info)
        {
            propertiesEditor.SetEditorInfo(info);
        }

        private void PropertiesEditor_BeginEdit(object? sender, EventArgs e)
        {
            BeginEdit?.Invoke(this, e);
        }

        private void PropertiesEditor_EndEdit(object? sender, EventArgs e)
        {
            //Part内のAnimationを変更した際にPartsを更新する
            //複数のアイテムを選択している場合にすべてのアイテムを更新するために必要
            if (DataContext is PartValueListViewModel vm)
                vm.CopyToOtherItems();

            EndEdit?.Invoke(this, e);
        }

        private void PointsEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is PartValueListViewModel oldVm)
            {
                oldVm.BeginEdit -= PropertiesEditor_BeginEdit;
                oldVm.EndEdit -= PropertiesEditor_EndEdit;
            }
            if (e.NewValue is PartValueListViewModel newVm)
            {
                newVm.BeginEdit += PropertiesEditor_BeginEdit;
                newVm.EndEdit += PropertiesEditor_EndEdit;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateButtons();
        }

        public void UpdateButtons()
        {
            if (DataContext is not PartValueListViewModel vm)
                return;

            if (vm.PartValues.Count == 0)
            {
                ShowAllPartsButton.IsEnabled = false;
                ShowOnlySelectedPartsButton.IsEnabled = false;
            }
            else
            {
                ShowAllPartsButton.IsEnabled = true;
            }

            List<PartValue> selecteds = GetSelecteds();
            ShowOnlySelectedPartsButton.IsEnabled = selecteds.Count > 0;

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
                EditTemplateButton.IsEnabled = false;
            }
            else
            {
                AddButton.IsEnabled = true;
                PasteButton.IsEnabled = _clipboad.Count > 0;
                ShowAllPartsButton.IsEnabled = true;

                if (selecteds.Count < 1)
                {
                    RemoveButton.IsEnabled = false;
                    UpButton.IsEnabled = false;
                    DownButton.IsEnabled = false;
                    CutButton.IsEnabled = false;
                    CopyButton.IsEnabled = false;
                    DuplicateButton.IsEnabled = false;
                    EditTemplateButton.IsEnabled = false;
                }
                else
                {
                    RemoveButton.IsEnabled = true;
                    CutButton.IsEnabled = true;
                    CopyButton.IsEnabled = true;
                    DuplicateButton.IsEnabled = true;
                    UpButton.IsEnabled = vm.CanMoveUpItem();
                    DownButton.IsEnabled = vm.CanMoveDownItem();
                    PartValue selected = selecteds[0];

                    if (selected.ControlledParameters.ThisLayerType == LayerType.Image
                        || selected.ControlledParameters.ThisLayerType == LayerType.Psd
                        || selected.ControlledParameters.ThisLayerType == LayerType.Video)
                    {
                        EditTemplateButton.IsEnabled = true;
                    }
                    else
                    {
                        EditTemplateButton.IsEnabled = false;
                    }
                }
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                if (e.Delta > 0)
                    scrollViewer.LineUp();
                else
                    scrollViewer.LineDown();

                e.Handled = true;
            }
        }

        private void List_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = FindVisualChild<ScrollViewer>(ItemList);
            if (scrollViewer == null) return;

            if (AnyOpendComboBox(ItemList))
            {
                e.Handled = false;
                return;
            }

            e.Handled = true;
            bool scrollingUp = e.Delta > 0;

            if ((scrollingUp && scrollViewer.VerticalOffset == 0) ||
                (!scrollingUp && scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight))
            {
                // 端に到達 → スクロールイベントを親に渡す

                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = UIElement.MouseWheelEvent,
                    Source = sender
                };

                // 親要素を取得してイベント再発火
                var parent = ((System.Windows.Controls.Control)sender).Parent as UIElement;
                parent?.RaiseEvent(eventArg);
            }
            else
            {
                // まだスクロール可能 → 自分で処理
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            }
        }


        public static bool AnyOpendComboBox(DependencyObject depObj)
        {
            if (depObj == null) return false;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                if (child is System.Windows.Controls.ComboBox combo)
                {
                    if (combo.IsDropDownOpen)
                        return true;
                }

                if (AnyOpendComboBox(child))
                    return true;
            }

            return false;
        }

        public static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is T t)
                    return t;
                else
                {
                    T? result = FindVisualChild<T>(child);

                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        private void ItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtons();
        }

        public List<PartValue> GetSelecteds()
        {
            List<PartValue> selecteds = [];
            foreach (var selected in ItemList.SelectedItems)
                if (selected is PartValue item)
                    selecteds.Add(item);
            return selecteds;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not PartValueListViewModel vm)
                return;

            if (_addingPartType == LayerType.Image || _addingPartType == LayerType.Psd || _addingPartType == LayerType.Video)
            {
                PartNameTree.Items.Clear();
                bool rootUnexist = !Path.Exists(vm.Root);

                if (!rootUnexist)
                {
                    try
                    {
                        DirectoryInfo di = new(vm.Root);

                        foreach (TreeViewItem item in CreateTreeItem(di))
                        {
                            PartNameTree.Items.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        SinTachieDialog.ShowError(ex);
                    }
                }

                if (PartNameTree.Items.Count == 0 || rootUnexist)
                {
                    var intro = rootUnexist
                        ? TextResource.PartValueListView_AddPartMessage_InvalidSourcePath
                        : TextResource.PartValueListView_AddPartMessage_NotFoundAnyFiles;
                    var dialog = SinTachieDialog.GetOKorCancel(intro + "\n" + TextResource.PartValueListView_AddPartMessage_AskAddEmptyPart);

                    if (dialog == DialogResult.OK)
                    {
                        var tmpSelectedIndex = vm.SelectedIndex;
                        BeginEdit?.Invoke(this, EventArgs.Empty);
                        string tagName = "[UnNamed]";
                        var tags = from pv1 in vm.PartValues select pv1.ControlledParameters.Tag;

                        if (tags.Contains(tagName))
                        {
                            int sideNum = 1;
                            while (tags.Contains($"{tagName}({sideNum})")) sideNum++;
                            tagName += $"({sideNum})";
                        }

                        PartValue pv2 = new();
                        ControlledParametersOfPart cp = pv2.ControlledParameters;
                        cp.Tag = tagName;
                        cp.SwitchType(_addingPartType);
                        vm.InsertItems([pv2]);
                    }
                }
            }
            else
            {
                PartValue pv3 = new();
                pv3.ControlledParameters.SwitchType(_addingPartType);
                vm.InsertItems([pv3]);
            }
        }

        IEnumerable<TreeViewItem> CreateTreeItem(DirectoryInfo target)
        {
            List<TreeViewItem> nodes = [];

            foreach (DirectoryInfo di in target.GetDirectories())
            {
                TreeViewItem t = new() { Header = di.Name };
                TreeViewItem[] tmp = [.. CreateTreeItem(di)];

                if (tmp.Length != 0)
                {
                    foreach (TreeViewItem ts in tmp)
                    {
                        t.Items.Add(ts);
                    }

                    if (_lastAddedPartPath.StartsWith(di.FullName))
                        t.IsExpanded = true;

                    nodes.Add(t);
                }
            }

            nodes.Sort(new NaturalStringComparer());
            IEnumerable<FileInfo> files = _addingPartType switch
            {
                LayerType.Image => target.GetFiles()
                        .Where(fi => FileSettings.Default.FileExtensions.GetFileType(fi.FullName) == FileType.画像)
                        .Where(fi => !(from c in fi.Name
                                       where c == '.'
                                       select c).Skip(1).Any()),
                LayerType.Psd => target.GetFiles()
                        .Where(fi => fi.Extension.Equals("psd", StringComparison.CurrentCultureIgnoreCase)),
                LayerType.Video => target.GetFiles()
                        .Where(fi => FileSettings.Default.FileExtensions.GetFileType(fi.FullName) == FileType.動画),
                _ => [],
            };
            IEnumerable<TreeViewItem> leafs = files.Select(fi =>
            {
                TreeViewItem t = new() { Header = fi.Name };
                t.MouseLeftButtonUp += PartTreeNodeClick;
                _nameTreeItems.Add((t, fi.FullName));
                return t;
            });
            List<TreeViewItem> leafs2 = [.. leafs];
            leafs2.Sort(new NaturalStringComparer());

            return [.. nodes, .. leafs2];
        }

        private void PartTreeNodeClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is not PartValueListViewModel vm)
                return;

            if (sender is TreeViewItem item)
            {
                string tagName;
                string partSourcePath = _nameTreeItems.Find(t => t.Item1 == item).Item2;
                string fromRoot = partSourcePath.Split(vm.Root + "\\").Last();

                if (Path.GetDirectoryName(fromRoot) is string dn && !string.IsNullOrEmpty(dn))
                {
                    tagName = dn;
                }
                else
                {
                    tagName = Path.GetFileNameWithoutExtension((string)item.Header);
                }

                string[] tags = [.. vm.PartValues.Select(pv => pv.ControlledParameters.Tag)];

                if (tags.Contains(tagName))
                {
                    int sideNum = 1;
                    while (tags.Contains($"{tagName}({sideNum})")) sideNum++;
                    tagName += $"({sideNum})";
                }

                PartValue pv = new();
                ControlledParametersOfPart cp = pv.ControlledParameters;
                cp.Tag = tagName;

                switch (_addingPartType)
                {
                    case LayerType.Image:
                        ((ImageFileParameter)cp.SourceSelectArg).ImageFilePath = partSourcePath;
                        break;

                    case LayerType.Psd:
                        cp.SwitchType(LayerType.Psd);
                        ((PsdFileParameter)cp.SourceSelectArg).PsdFilePath = partSourcePath;
                        break;

                    case LayerType.Video:
                        cp.SwitchType(LayerType.Video);
                        ((VideoFileParameter)cp.SourceSelectArg).VideoFilePath = partSourcePath;
                        break;

                    default:
                        return;
                }

                vm.InsertItems([pv]);
                _lastAddedPartPath = partSourcePath;
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not PartValueListViewModel vm)
                return;

            vm.RemoveItems(GetSelecteds());
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not PartValueListViewModel vm)
                return;

            vm.MoveUpItem();
            UpdateButtons();
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not PartValueListViewModel vm)
                return;

            vm.MoveDownItem();
            UpdateButtons();
        }

        private void EditTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            List<PartValue> selecteds = GetSelecteds();

            if (selecteds.Count != 1)
                return;

            PartValue selected = selecteds[0];

            if (selected.ControlledParameters.ThisLayerType == LayerType.Image
                || selected.ControlledParameters.ThisLayerType == LayerType.Psd
                || selected.ControlledParameters.ThisLayerType == LayerType.Video)
            {
                ControlledParametersOfPart param = selected.ControlledParameters;
                string sourcePath;
                if (param.SourceSelectArg is ImageFileParameter ip)
                    sourcePath = ip.ImageFilePath;
                else if (param.SourceSelectArg is PsdFileParameter pp)
                    sourcePath = pp.PsdFilePath;
                else
                    sourcePath = ((VideoFileParameter)param.SourceSelectArg).VideoFilePath;

                PartValueTemplateEditorDialog dialog = new();
                dialog.SetSourcePath(sourcePath);

                if (dialog.SelectedTemplate is null)
                    return;

                selected.ControlledParameters = new(dialog.SelectedTemplate.Param);
            }
        }

        private void CutButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not PartValueListViewModel vm)
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
            if (DataContext is not PartValueListViewModel vm)
                return;

            vm.InsertItems(GetCloneOfClipboad());
        }

        private void DuplicateButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not PartValueListViewModel vm)
                return;

            vm.InsertItems(GetCloneOfSelected());
        }

        private void ShowAllPartsButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartValueListViewModel vm)
                vm.ShowAllParts();
        }

        private void ShowOnlySelectedPartsButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartValueListViewModel vm)
                vm.ShowOnlySelectedParts(GetSelecteds());
        }

        private void SelectLayerTypeButton_Click(object sender, RoutedEventArgs e)
        {
            LayerTypeSelectorPopup.IsOpen = true;
        }

        private void SelectImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _addingPartType = LayerType.Image;
        }

        private void SelectPsd_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _addingPartType = LayerType.Psd;
        }

        private void SelectVideo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _addingPartType = LayerType.Video;
        }

        private void SelectScene_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _addingPartType = LayerType.Scene;
        }

        private void SelectGroup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _addingPartType = LayerType.Group;
        }

        private List<PartValue> GetCloneOfSelected()
        {
            if (DataContext is not PartValueListViewModel vm)
                return [];

            HashSet<PartValue> hash = [];

            foreach (PartValue pv1 in GetSelecteds())
            {
                List<PartValue> list1 = [pv1];
                List<PartValue> list2 = [];

                while (list1.Count > 0)
                {
                    foreach (PartValue pv2 in list1)
                    {
                        if (pv2.ControlledParameters.ThisLayerType == Enums.LayerType.Group)
                            list2.AddRange(vm.GetDescendants(pv2));

                        hash.Add(pv2);
                    }

                    list1 = list2;
                    list2 = [];
                }
            }

            List<PartValue> list3 = [.. hash];
            list3.Sort((a, b) => vm.PartValues.IndexOf(a).CompareTo(vm.PartValues.IndexOf(b)));
            List<(PartValue, PartValue)> list4 = [];

            foreach (PartValue pv1 in list3)
            {
                if (pv1.ParentIndex < 0 || pv1.ParentIndex >= vm.PartValues.Count)
                {
                    PartValue pv2 = new(pv1) { ParentIndex = -1 };
                    list4.Add((pv1, pv2));
                }
                else
                {
                    if (list4.Select(item => item.Item1).FirstOrDefault(mi2 => mi2 == vm.PartValues[pv1.ParentIndex]) is PartValue mi3)
                    {
                        PartValue pv2 = new(pv1)
                        {
                            ParentIndex = list4.Select(item => item.Item1).ToList().IndexOf(mi3)
                        };
                        list4.Add((pv1, pv2));
                    }
                    else
                    {
                        PartValue pv2 = new(pv1) { ParentIndex = -1 };
                        list4.Add((pv1, pv2));
                    }
                }
            }

            return [.. list4.Select(item => item.Item2)];
        }

        private List<PartValue> GetCloneOfClipboad()
        {
            return [.. _clipboad.Select(pv => new PartValue(pv))];
        }
    }
}
