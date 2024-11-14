using SinTachiePlugin.Informations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Settings;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SinTachiePlugin.Parts
{
    public abstract class PartsListControllerViewModelBase : Bindable, INotifyPropertyChanged, IPropertyEditorControl, IDisposable
    {
        readonly INotifyPropertyChanged item;
        protected readonly ItemProperty[] properties;

        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public ImmutableList<PartBlock> Parts { get => parts; set => Set(ref parts, value); }
        ImmutableList<PartBlock> parts = [];

        public bool PopupIsOpen
        {
            get => popupIsOpen;
            set
            {
                //if (!value)
                //{
                //    SelectedAddingPartIndex = -1;
                //}
                Set(ref popupIsOpen, value);
            }
        }
        bool popupIsOpen = false;

        public int SelectedIndex { get => selectedIndex; set => Set(ref selectedIndex, value); }
        int selectedIndex = -1;

        public string Root
        {
            get => root;
            set => Set(ref root, value);
        }
        string root = string.Empty;

        public List<PartNameTreeNode> PartNameTree { get => partNameTreeNode; set => Set(ref partNameTreeNode, value); }
        List<PartNameTreeNode> partNameTreeNode = [];
        public object SelectedTreeViewItem
        {
            get => _selectedTreeViewItem;
            set
            {
                _selectedTreeViewItem = value;
                if (value is PartNameTreeNode newSelected)
                {
                    try
                    {
                        if (!newSelected.Children.Any() && !AddingNow)
                        {
                            AddingNow = true;
                            string partImagePath = newSelected.FullName;
                            string? dn = Path.GetDirectoryName(partImagePath);
                            if (dn == null) throw new Exception("選択されたファイルからディレクトリのパスを取得できませんでした。");
                            string tag;
                            if (dn == Root)
                            {
                                tag = TagOfIndependent;
                            }
                            else
                            {
                                var splited = dn.Split(Root + "\\");
                                if (splited.Length != 2) throw new Exception("選択されたファイルと「素材の場所」フォルダとの関係を正しく取得できませんでした。");
                                var SelectedAddingPart = splited[1];
                                tag = SelectedAddingPart;
                            }
                            var tags = from part in Parts select part.TagName;
                            if (tags.Contains(tag))
                            {
                                int sideNum = 1;
                                while (tags.Contains($"{tag}({sideNum})")) sideNum++;
                                tag += $"({sideNum})";
                            }
                            int tmpSelectedIndex;
                            if (SelectedIndex < 0)
                            {
                                tmpSelectedIndex = Parts.Count;
                                Parts = Parts.Add(new PartBlock(partImagePath, tag, tags.ToArray()));
                                SetProparties();
                            }
                            else
                            {
                                tmpSelectedIndex = SelectedIndex;
                                Parts = Parts.Insert(tmpSelectedIndex, new PartBlock(partImagePath, tag, tags.ToArray()));
                                SetProparties();
                            }
                            EndEdit?.Invoke(this, EventArgs.Empty);
                            SelectedIndex = tmpSelectedIndex;
                            PopupIsOpen = false;
                            AddingNow = false;
                        }
                        Set(ref _selectedTreeViewItem, value);
                    }
                    catch (Exception e)
                    {
                        SinTachieDialog.ShowWarning("パーツ追加時にエラーが発生しました" +
                            "\n" + e.Message);
                    }
                } 
            }
        }
        private object _selectedTreeViewItem = new PartNameTreeNode();

        bool AddingNow = false;

        private static readonly string TagOfIndependent = "(無所属)";

        private bool RootUnexist = false;

        public ActionCommand AddCommand { get; }
        public ActionCommand RemoveCommand { get; }
        public ActionCommand DuplicationCommand { get; }
        public ActionCommand MoveUpCommand { get; }
        public ActionCommand MoveDownCommand { get; }
        public ActionCommand WriteDefaultCommand { get; }
        public ActionCommand DeleteDefaultCommand { get; }
        public ActionCommand ReloadDefaultCommand { get; }

        public PartsListControllerViewModelBase(ItemProperty[] properties)
        {
            this.properties = properties;

            item = (INotifyPropertyChanged)properties[0].PropertyOwner;
            item.PropertyChanged += Item_PropertyChanged;

            AddCommand = new ActionCommand(
                _ => true,
                _ =>
                {
                    RootUnexist = !Path.Exists(Root);
                    if (RootUnexist)
                    {
                        PartNameTree = new();
                    }
                    else
                    {
                        DirectoryInfo di = new(Root);
                        PartNameTree = new PartNameTreeNode(di).Children;
                    }

                    if(PartNameTree.Count == 0 || RootUnexist)
                    {
                        var intro = RootUnexist ? "素材の場所のパスが無効です。" : "素材の場所にパーツが見つかりませんでした。";
                        var dialog = SinTachieDialog.GetDialog($"{intro}\n画像未指定のパーツブロックを追加しますか？");
                        if(dialog == DialogResult.OK)
                        {
                            var tmpSelectedIndex = SelectedIndex;
                            BeginEdit?.Invoke(this, EventArgs.Empty);
                            string tag = "[Untitled]";
                            var tags = from part in Parts select part.TagName;
                            if (tags.Contains(tag))
                            {
                                int sideNum = 1;
                                while (tags.Contains($"{tag}({sideNum})")) sideNum++;
                                tag += $"({sideNum})";
                            }
                            Parts = Parts.Insert(tmpSelectedIndex + 1, new PartBlock("", tag, tags.ToArray()));
                            SetProparties();
                            EndEdit?.Invoke(this, EventArgs.Empty);
                            SelectedIndex = tmpSelectedIndex + 1;
                            PopupIsOpen = false;
                        }
                        return;
                    }
                    PopupIsOpen = true;
                });

            RemoveCommand = new ActionCommand(
                _ => parts.Count > 0 && SelectedIndex > -1,
                _ =>
                {
                    var tmpSelectedIndex = SelectedIndex;
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    Parts = Parts.RemoveAt(tmpSelectedIndex);
                    SetProparties();
                    EndEdit?.Invoke(this, EventArgs.Empty);
                    if (parts.Count > 0) SelectedIndex = Math.Min(tmpSelectedIndex, parts.Count - 1);
                });

            DuplicationCommand = new ActionCommand(
                _ => SelectedIndex > -1,
                _ =>
                {
                    var tmpSelectedIndex = SelectedIndex;
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    var copied = new PartBlock(Parts[SelectedIndex]);
                    Parts = Parts.Insert(tmpSelectedIndex + 1, copied);
                    SetProparties();
                    EndEdit?.Invoke(this, EventArgs.Empty);
                    SelectedIndex = tmpSelectedIndex + 1;
                });

            MoveUpCommand = new ActionCommand(
                _ => SelectedIndex > 0,
                _ =>
                {
                    var tmpSelectedIndex = SelectedIndex;
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    var clone = Parts[SelectedIndex];
                    Parts = Parts.RemoveAt(tmpSelectedIndex);
                    Parts = Parts.Insert(tmpSelectedIndex - 1, clone);
                    SetProparties();
                    EndEdit?.Invoke(this, EventArgs.Empty);
                    SelectedIndex = tmpSelectedIndex - 1;
                });

            MoveDownCommand = new ActionCommand(
                _ => SelectedIndex < parts.Count - 1 && SelectedIndex > -1,
                _ =>
                {
                    var tmpSelectedIndex = SelectedIndex;
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    var clone = parts[SelectedIndex];
                    Parts = Parts.RemoveAt(tmpSelectedIndex);
                    Parts = Parts.Insert(tmpSelectedIndex + 1, clone);
                    SetProparties();
                    EndEdit?.Invoke(this, EventArgs.Empty);
                    SelectedIndex = tmpSelectedIndex + 1;
                });

            WriteDefaultCommand = new ActionCommand(
                _ => SelectedIndex > -1,
                _ =>
                {
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    var selected = Parts[SelectedIndex];
                    if (selected == null)
                    {
                        string clsName = GetType().Name;
                        string? mthName = MethodBase.GetCurrentMethod()?.Name;
                        SinTachieDialog.ShowError("選択されたブロックを取得できません。", clsName, mthName);
                        return;
                    }
                    selected.UpdateDefault();
                    EndEdit?.Invoke(this, EventArgs.Empty);
                });

            DeleteDefaultCommand = new ActionCommand(
                _ => SelectedIndex > -1,
                _ =>
                {
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    var selected = Parts[SelectedIndex];
                    if (selected == null)
                    {
                        string clsName = GetType().Name;
                        string? mthName = MethodBase.GetCurrentMethod()?.Name;
                        SinTachieDialog.ShowError("選択されたブロックを取得できません。", clsName, mthName);
                        return;
                    }
                    selected.DeleteDafault();
                    EndEdit?.Invoke(this, EventArgs.Empty);
                });

            ReloadDefaultCommand = new ActionCommand(
                _ => SelectedIndex > -1,
                _ =>
                {
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    var selected = Parts[SelectedIndex];
                    if (selected == null)
                    {
                        string clsName = GetType().Name;
                        string? mthName = MethodBase.GetCurrentMethod()?.Name;
                        SinTachieDialog.ShowError("選択されたブロックを取得できません。", clsName, mthName);
                        return;
                    }
                    SetProparties();
                    selected.ReloadDefault();
                    EndEdit?.Invoke(this, EventArgs.Empty);
                });

            UpdateProperties();
        }

        protected abstract void SetProparties();

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == properties[0].PropertyInfo.Name)
                UpdateProperties();
        }

        protected abstract void UpdateParts();

        protected void UpdateProperties()
        {
            UpdateParts();

            var commands = new[] { AddCommand, RemoveCommand, DuplicationCommand, MoveUpCommand, MoveDownCommand, ReloadDefaultCommand };
            foreach (var command in commands)
                command.RaiseCanExecuteChanged();
        }

        public void CopyToOtherItems()
        {
            //現在のアイテムの内容を他のアイテムにコピーする
            var otherProperties = properties.Skip(1);
            foreach (var property in otherProperties)
                property.SetValue(Parts.Select(x => new PartBlock(x)).ToImmutableList());
        }

        public void Dispose()
        {
            item.PropertyChanged -= Item_PropertyChanged;
        }
    }
}
