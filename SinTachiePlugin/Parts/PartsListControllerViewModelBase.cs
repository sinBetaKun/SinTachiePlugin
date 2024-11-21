using Newtonsoft.Json;
using SinTachiePlugin.Informations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
        static PartBlock? clipedBlock = null;

        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        /// <summary>
        /// パーツブロックのリストの内容
        /// </summary>
        public ImmutableList<PartBlock> Parts { get => parts; set => Set(ref parts, value); }
        ImmutableList<PartBlock> parts = [];

        /// <summary>
        /// リストで選択されているパーツブロックのインデックス
        /// </summary>
        public int SelectedPartIndex
        {
            get => selectedPartIndex;
            set
            {
                SomeBlockSelected = value > -1;
                Set(ref selectedPartIndex, value);
            }
        }
        int selectedPartIndex = -1;

        /// <summary>
        /// リスト内のいずれかのパーツブロックが選択されている状態か否か
        /// </summary>
        public bool SomeBlockSelected { get => someBlockSelected; set => Set(ref someBlockSelected, value); }
        bool someBlockSelected = false;

        /// <summary>
        /// 右クリックメニューで「切り取り」を選択したときの処理
        /// </summary>
        public void ScissorsFunc()
        {
            BeginEdit?.Invoke(this, EventArgs.Empty);
            CopyFunc();
            RemovePartBlock();
            EndEdit?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 右クリックメニューで「コピー」を選択したときの処理
        /// </summary>
        public void CopyFunc()
        {
            clipedBlock = new(Parts[SelectedPartIndex]);
        }

        /// <summary>
        /// 右クリックメニューで「貼り付け」が選択可能か否か
        /// </summary>
        public bool PasteEnable { get => pasteEnable; set => Set(ref pasteEnable, value); }
        bool pasteEnable = false;

        /// <summary>
        /// 右クリックメニューで「貼り付け」を選択したときの処理
        /// </summary>
        public void PasteFunc()
        {
            if (clipedBlock == null)
            {
                string className = GetType().Name;
                string? mthName = MethodBase.GetCurrentMethod()?.Name;
                SinTachieDialog.ShowError("疑似クリップボードにnullが代入されている状態での貼り付け処理は、本来なら不可能な処理です。",
                    className, mthName);
                return;
            }

            var tmpSelectedIndex = SelectedPartIndex;
            BeginEdit?.Invoke(this, EventArgs.Empty);
            if (tmpSelectedIndex < 0)
            {
                Parts = Parts.Add(new PartBlock(clipedBlock));
                tmpSelectedIndex = Parts.Count - 1;
            }
            else
            {
                Parts = Parts.Insert(tmpSelectedIndex, new PartBlock(clipedBlock));
            }
            SetProperties();
            SelectedPartIndex = tmpSelectedIndex;

            EndEdit?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 右クリックメニューで「複製」を選択したときの処理
        /// </summary>
        public void DuplicationFunc()
        {
            BeginEdit?.Invoke(this, EventArgs.Empty);
            var tmpSelectedIndex = SelectedPartIndex;
            var copied = new PartBlock(Parts[SelectedPartIndex]);
            Parts = Parts.Insert(tmpSelectedIndex, new PartBlock(copied));
            SetProperties();
            SelectedPartIndex = tmpSelectedIndex + 1;
            EndEdit?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 右クリックメニューで「削除」を選択したときの処理
        /// </summary>
        public void RemoveFunc()
        {
            BeginEdit?.Invoke(this, EventArgs.Empty);
            RemovePartBlock();
            EndEdit?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 右クリックメニューが開かれている状態か否か
        /// </summary>
        public bool ContextMenuIsOpen
        {
            get => contextMeneIsOpen;
            set
            {
                PasteEnable = clipedBlock != null;
                Set(ref contextMeneIsOpen, value);
            }
        }
        bool contextMeneIsOpen = false;

        /// <summary>
        /// DirectorySelector「素材の場所」のパス
        /// </summary>
        public string Root
        {
            get => root;
            set => Set(ref root, value);
        }
        string root = string.Empty;

        /// <summary>
        /// パーツ選択ツリーの内容
        /// </summary>
        public List<PartNameTreeNode> PartNameTree { get => partNameTreeNode; set => Set(ref partNameTreeNode, value); }
        List<PartNameTreeNode> partNameTreeNode = [];

        /// <summary>
        /// パーツ選択ツリーが表示されているか否か
        /// </summary>
        public bool PartsPopupIsOpen { get => partsPopupIsOpen; set => Set(ref partsPopupIsOpen, value); }
        bool partsPopupIsOpen = false;

        /// <summary>
        /// パーツ選択ツリーの選択されている項目
        /// </summary>
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
                        if (newSelected.Children.Count == 0)
                        {
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
                            BeginEdit?.Invoke(this, EventArgs.Empty);
                            if (SelectedPartIndex < 0)
                            {
                                tmpSelectedIndex = Parts.Count;
                                Parts = Parts.Add(new PartBlock(partImagePath, tag, tags.ToArray()));
                                SetProperties();
                            }
                            else
                            {
                                tmpSelectedIndex = SelectedPartIndex;
                                Parts = Parts.Insert(tmpSelectedIndex, new PartBlock(partImagePath, tag, tags.ToArray()));
                                SetProperties();
                            }
                            EndEdit?.Invoke(this, EventArgs.Empty);
                            SelectedPartIndex = tmpSelectedIndex;
                            PartsPopupIsOpen = false;
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

        private static readonly string TagOfIndependent = "(無所属)";

        private bool RootUnexist = false;

        public ActionCommand AddCommand { get; }
        public ActionCommand RemoveCommand { get; }
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

                    if (PartNameTree.Count == 0 || RootUnexist)
                    {
                        var intro = RootUnexist ? "素材の場所のパスが無効です。" : "素材の場所にパーツが見つかりませんでした。";
                        var dialog = SinTachieDialog.GetDialog($"{intro}\n画像未指定のパーツブロックを追加しますか？");
                        if (dialog == DialogResult.OK)
                        {
                            var tmpSelectedIndex = SelectedPartIndex;
                            BeginEdit?.Invoke(this, EventArgs.Empty);
                            string tag = "[Untitled]";
                            var tags = from part in Parts select part.TagName;
                            if (tags.Contains(tag))
                            {
                                int sideNum = 1;
                                while (tags.Contains($"{tag}({sideNum})")) sideNum++;
                                tag += $"({sideNum})";
                            }
                            if (tmpSelectedIndex < 0)
                            {
                                Parts = Parts.Add(new PartBlock("", tag, tags.ToArray()));
                                SelectedPartIndex = Parts.Count - 1;
                            }
                            else
                            {
                                Parts = Parts.Insert(tmpSelectedIndex, new PartBlock("", tag, tags.ToArray()));
                                SelectedPartIndex = tmpSelectedIndex;
                            }
                            SetProperties();
                            EndEdit?.Invoke(this, EventArgs.Empty);
                            PartsPopupIsOpen = false;
                        }
                        return;
                    }
                    PartsPopupIsOpen = true;
                });

            RemoveCommand = new ActionCommand(
                _ => parts.Count > 0 && SelectedPartIndex > -1,
                _ =>
                {
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    RemovePartBlock();
                    EndEdit?.Invoke(this, EventArgs.Empty);
                });

            MoveUpCommand = new ActionCommand(
                _ => SelectedPartIndex > 0,
                _ =>
                {
                    var tmpSelectedIndex = SelectedPartIndex;
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    var clone = Parts[SelectedPartIndex];
                    Parts = Parts.RemoveAt(tmpSelectedIndex);
                    Parts = Parts.Insert(tmpSelectedIndex - 1, clone);
                    SetProperties();
                    EndEdit?.Invoke(this, EventArgs.Empty);
                    SelectedPartIndex = tmpSelectedIndex - 1;
                });

            MoveDownCommand = new ActionCommand(
                _ => SelectedPartIndex < parts.Count - 1 && SelectedPartIndex > -1,
                _ =>
                {
                    var tmpSelectedIndex = SelectedPartIndex;
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    var clone = parts[SelectedPartIndex];
                    Parts = Parts.RemoveAt(tmpSelectedIndex);
                    Parts = Parts.Insert(tmpSelectedIndex + 1, clone);
                    SetProperties();
                    EndEdit?.Invoke(this, EventArgs.Empty);
                    SelectedPartIndex = tmpSelectedIndex + 1;
                });

            WriteDefaultCommand = new ActionCommand(
                _ => SelectedPartIndex > -1,
                _ =>
                {
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    var selected = Parts[SelectedPartIndex];
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
                _ => SelectedPartIndex > -1,
                _ =>
                {
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    var selected = Parts[SelectedPartIndex];
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
                _ => SelectedPartIndex > -1,
                _ =>
                {
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    var selected = Parts[SelectedPartIndex];
                    var tmpSelectedPartIndex = SelectedPartIndex;
                    if (selected == null)
                    {
                        string clsName = GetType().Name;
                        string? mthName = MethodBase.GetCurrentMethod()?.Name;
                        SinTachieDialog.ShowError("選択されたブロックを取得できません。", clsName, mthName);
                        return;
                    }
                    selected.ReloadDefault();
                    SetProperties();
                    SelectedPartIndex = tmpSelectedPartIndex;
                    EndEdit?.Invoke(this, EventArgs.Empty);
                });


            UpdateProperties();
        }

        private void RemovePartBlock()
        {
            var tmpSelectedIndex = SelectedPartIndex;
            Parts = Parts.RemoveAt(tmpSelectedIndex);
            SetProperties();
            if (Parts.Count > 0) SelectedPartIndex = Math.Min(tmpSelectedIndex, Parts.Count - 1);
            else SelectedPartIndex = -1;
        }

        public abstract void SetProperties();

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == properties[0].PropertyInfo.Name)
                UpdateProperties();
        }

        protected abstract void UpdateParts();

        protected void UpdateProperties()
        {
            UpdateParts();

            var commands = new[] { AddCommand, RemoveCommand, MoveUpCommand, MoveDownCommand, ReloadDefaultCommand };
            foreach (var command in commands)
                command.RaiseCanExecuteChanged();
        }

        public abstract void CopyToOtherItems();

        public void Dispose()
        {
            item.PropertyChanged -= Item_PropertyChanged;
        }
    }
}
