using Newtonsoft.Json;
using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using SinTachiePlugin.Part;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter;
using SinTachiePlugin.Properties;
using SinTachiePlugin.Window_Stp.PartValueTemplateEditor;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project.Items;
using YukkuriMovieMaker.Settings;

namespace SinTachiePlugin.Control.PartValueList
{
    /// <summary>
    /// PartValueListView.xaml の相互作用ロジック
    /// </summary>
    public partial class PartValueListView : UserControl, IPropertyEditorControl2
    {
        private class NaturalStringComparer : IComparer<TreeViewItem>
        {
            [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
            static extern int StrCmpLogicalW(string x, string y);

            public int Compare(TreeViewItem x, TreeViewItem y) => StrCmpLogicalW((string)x.Header, (string)y.Header);
        }

        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        private static string _clipboad = "[]";
        private static int _selectedInClip = -1;
        
        private static JsonSerializerSettings GetJsonSetting =>
            new() { TypeNameHandling = TypeNameHandling.Auto };
        
        private static IEnumerable<PartValue> GetClipped() {
            if (JsonConvert.DeserializeObject<IEnumerable<PartValue>>(_clipboad, GetJsonSetting) is IEnumerable<PartValue> pv_e0)
                return pv_e0;

            SinTachieDialog.ShowWarning(TextResource.ErrorMessage_FailToGetClippedFromJson);
            return [];
        }

        private static void SetClipped(IEnumerable<PartValue> pvs)
        {
            _clipboad = JsonConvert.SerializeObject(pvs, Formatting.Indented, GetJsonSetting);
        }


        private LayerType _addingPartType = LayerType.Image;

        private string _lastAddedPartPath = string.Empty;
        private readonly List<(TreeViewItem, string)> _nameTreeItems = [];

        public PartValueListView()
        {
            InitializeComponent();
            DataContextChanged += PointsEditor_DataContextChanged;
        }

        public void SetEditorInfo(IEditorInfo? info)
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
            ItemMovable cmv = vm.CanMoveItem(selecteds);
            UpButton.IsEnabled = cmv == ItemMovable.OnlyUp || cmv == ItemMovable.Both;
            DownButton.IsEnabled = cmv == ItemMovable.OnlyDown || cmv == ItemMovable.Both;

            if (cmv == ItemMovable.Hazard)
            {
                HazardIcon.ToolTip = TextResource.PartValueListView_WarningA;
                HazardIcon.Visibility = Visibility.Visible;
            }
            else
            {
                HazardIcon.ToolTip = string.Empty;
                HazardIcon.Visibility = Visibility.Hidden;
            }

            if (selecteds.Count > 1)
            {
                AddButton.IsEnabled = false;
                RemoveButton.IsEnabled = true;
                CutButton.IsEnabled = true;
                CopyButton.IsEnabled = true;
                PasteButton.IsEnabled = false;
                DuplicateButton.IsEnabled = false;
                EditTemplateButton.IsEnabled = false;
            }
            else
            {
                AddButton.IsEnabled = true;
                PartValue[] pv_a0 = [.. GetClipped()];
                PasteButton.IsEnabled = pv_a0.Length > 0;
                ShowAllPartsButton.IsEnabled = true;

                if (selecteds.Count < 1)
                {
                    RemoveButton.IsEnabled = false;
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
                    DuplicateButton.IsEnabled = cmv != ItemMovable.Hazard;
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
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 120);
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

        private void UpdateSelecting()
        {
            if (DataContext is not PartValueListViewModel vm)
                return;

            ItemList.SelectedItems.Clear();

            foreach (int i_1 in vm.SelectedIndexes)
                ItemList.SelectedItems.Add(ItemList.Items[i_1]);
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
                bool rootUnexist = !vm.Assets.All(x => Path.Exists(x.Path));

                if (!rootUnexist)
                {
                    try
                    {
                        //DirectoryInfo di = new(vm.Root);

                        //foreach (TreeViewItem item in CreateTreeItem(di))
                        //{
                        //    PartNameTree.Items.Add(item);
                        //}
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
                    MessageBoxResult dialog = SinTachieDialog.GetOKorCancel(intro + "\n" + TextResource.PartValueListView_AddPartMessage_AskAddEmptyPart);

                    if (dialog == MessageBoxResult.OK)
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
                        vm.InsertItems([pv2], pv2);
                        UpdateSelecting();
                    }
                }
            }
            else
            {
                PartValue pv3 = new();
                pv3.ControlledParameters.SwitchType(_addingPartType);
                vm.InsertItems([pv3], pv3);
                UpdateSelecting();
            }

            UpdateButtons();
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

            //if (sender is TreeViewItem item)
            //{
            //    string tagName;
            //    string partSourcePath = _nameTreeItems.Find(t => t.Item1 == item).Item2;
            //    string fromRoot = partSourcePath.Split(vm.Root + "\\").Last();

            //    if (Path.GetDirectoryName(fromRoot) is string dn && !string.IsNullOrEmpty(dn))
            //    {
            //        tagName = dn;
            //    }
            //    else
            //    {
            //        tagName = Path.GetFileNameWithoutExtension((string)item.Header);
            //    }

            //    string[] tags = [.. vm.PartValues.Select(pv => pv.ControlledParameters.Tag)];

            //    if (tags.Contains(tagName))
            //    {
            //        int sideNum = 1;
            //        while (tags.Contains($"{tagName}({sideNum})")) sideNum++;
            //        tagName += $"({sideNum})";
            //    }

            //    PartValue pv = new();
            //    ControlledParametersOfPart cp = pv.ControlledParameters;
            //    cp.Tag = tagName;

            //    switch (_addingPartType)
            //    {
            //        case LayerType.Image:
            //            ((ImageFileParameter)cp.SourceSelectArg).ImageFilePath = partSourcePath;
            //            break;

            //        case LayerType.Psd:
            //            cp.SwitchType(LayerType.Psd);
            //            ((PsdFileParameter)cp.SourceSelectArg).PsdFileInfo.FilePath = partSourcePath;
            //            break;

            //        case LayerType.Video:
            //            cp.SwitchType(LayerType.Video);
            //            ((VideoFileParameter)cp.SourceSelectArg).VideoFilePath = partSourcePath;
            //            break;

            //        default:
            //            return;
            //    }

            //    vm.InsertItems2([pv]);
            //    _lastAddedPartPath = partSourcePath;
            //}
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

            vm.MoveItems(GetSelecteds(), (PartValue)ItemList.SelectedItem, true);
            UpdateSelecting();
            UpdateButtons();
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not PartValueListViewModel vm)
                return;

            vm.MoveItems(GetSelecteds(), (PartValue)ItemList.SelectedItem, false);
            UpdateSelecting();
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
                    sourcePath = pp.PsdFileInfo.FilePath ?? string.Empty;
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

            (List<PartValue> pv_l0, int i_0) = GetCloneOfSelected();
            SetClipped(pv_l0);
            _selectedInClip = i_0;
            vm.RemoveItems(GetSelecteds());
            ItemList.SelectedIndex = -1;
            UpdateButtons();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            (List<PartValue> pv_l0, int i_0) = GetCloneOfSelected();
            SetClipped(pv_l0);
            _selectedInClip = i_0;
            UpdateButtons();
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not PartValueListViewModel vm)
                return;

            List<PartValue> pv_l0 = [.. GetClipped()];
            PartValue lastSelected = pv_l0[_selectedInClip];
            vm.InsertItems(pv_l0, lastSelected);
            UpdateSelecting();
            UpdateButtons();
        }

        private void DuplicateButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not PartValueListViewModel vm)
                return;

            ItemList.SelectedIndex = GetSelecteds().Max(ItemList.Items.IndexOf);
            (List<PartValue> pv_l0, int i_0) = GetCloneOfSelected();
            vm.InsertItems(pv_l0, pv_l0[i_0]);
            UpdateSelecting();
            UpdateButtons();
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

        private (List<PartValue>, int) GetCloneOfSelected()
        {
            if (DataContext is not PartValueListViewModel vm)
                return ([], -1);

            List<PartValue> pv_l0 = [];
            PartValue? lastSelected = (PartValue)ItemList.SelectedItem;

            foreach (PartValue pv_0 in GetSelecteds())
            {
                List<PartValue> pv_l1 = [pv_0];
                List<PartValue> pv_l2 = [];

                while (pv_l1.Count > 0)
                {
                    foreach (PartValue pv_1 in pv_l1)
                    {
                        if (pv_1.ControlledParameters.ThisLayerType == LayerType.Group)
                            pv_l2.AddRange(vm.GetDescendants(pv_1));

                        pv_l0.Add(pv_1);
                    }

                    pv_l1 = pv_l2;
                    pv_l2 = [];
                }
            }

            List<PartValue> pv_l3 = [.. pv_l0.Distinct().OrderBy(vm.PartValues.IndexOf)];
            int i_0 = pv_l3.IndexOf(lastSelected);
            List<(PartValue, PartValue)> tp_l0 = [];

            foreach (PartValue pv_0 in pv_l3)
            {
                if (pv_0.ParentIndex < 0 || pv_0.ParentIndex >= vm.PartValues.Count)
                {
                    PartValue pv_1 = new(pv_0) { ParentIndex = -1 };
                    tp_l0.Add((pv_0, pv_1));
                }
                else
                {
                    if (tp_l0.Select(item => item.Item1).FirstOrDefault(x => x == vm.PartValues[pv_0.ParentIndex]) is PartValue pv_1)
                    {
                        PartValue pv_2 = new(pv_0)
                        {
                            ParentIndex = tp_l0.Select(item => item.Item1).ToList().IndexOf(pv_1)
                        };
                        tp_l0.Add((pv_0, pv_2));
                    }
                    else
                    {
                        PartValue pv2 = new(pv_0) { ParentIndex = -1 };
                        tp_l0.Add((pv_0, pv2));
                    }
                }
            }

            return ([.. tp_l0.Select(item => item.Item2)], i_0);
        }

        private void PartValueView_GroupPullDownChanged(object sender, EventArgs e)
        {
            if (DataContext is not PartValueListViewModel vm)
                return;

            if (sender is not PartValueView pvv)
                return;

            if (pvv.DataContext is not PartValue pv)
                return;

            if (pv.ControlledParameters.SourceSelectArg is not GroupParameter)
                return;

            vm.UpdateSource();
        }
    }
}
