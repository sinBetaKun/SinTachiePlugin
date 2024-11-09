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
                if (!value)
                {
                    SelectedAddingPartIndex = -1;
                }
                Set(ref popupIsOpen, value);
            }
        }
        bool popupIsOpen = false;

        public int SelectedIndex { get => selectedIndex; set => Set(ref selectedIndex, value); }
        int selectedIndex = -1;

        public string Root
        {
            get => root;
            set
            {
                Set(ref root, value);
                UpdatePopupListSource();
            }
        }
        string root = string.Empty;

        public string[] PopupListSource { get => popupListSource; set => Set(ref popupListSource, value); }
        string[] popupListSource = [];

        bool AddingNow = false;

        public int SelectedAddingPartIndex
        {
            get => selectedAddingPartIndex;
            set
            {
                if (value > -1 && !AddingNow)
                {
                    AddingNow = true;
                    var SelectedAddingPart = PopupListSource[value];
                    var tmpSelectedIndex = SelectedIndex;
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    bool independentSelected = independentExist && (value == 0);
                    string partImagePath = FindFirstImageOfPart(independentSelected ? string.Empty : SelectedAddingPart);
                    string tag = independentSelected ? TagOfIndependent : SelectedAddingPart;
                    var tags = from part in Parts select part.TagName;
                    if (tags.Contains(tag))
                    {
                        int sideNum = 1;
                        while (tags.Contains($"{tag}({sideNum})")) sideNum++;
                        tag += $"({sideNum})";
                    }
                    Parts = Parts.Insert(tmpSelectedIndex + 1, new PartBlock(partImagePath, tag, tags.ToArray()));
                    SetProparties();
                    EndEdit?.Invoke(this, EventArgs.Empty);
                    SelectedIndex = tmpSelectedIndex + 1;
                    PopupIsOpen = false;
                    AddingNow = false;
                }
                Set(ref selectedAddingPartIndex, value);
            }
        }
        int selectedAddingPartIndex = -1;

        private static readonly string TagOfIndependent = "(無所属)";
        bool independentExist = false;

        public ActionCommand AddCommand { get; }
        public ActionCommand RemoveCommand { get; }
        public ActionCommand DuplicationCommand { get; }
        public ActionCommand MoveUpCommand { get; }
        public ActionCommand MoveDownCommand { get; }
        //public ActionCommand RelodeImageCommand { get; }
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
                    UpdatePopupListSource();
                    if(PopupListSource.Length < 1)
                    {
                        var dialog = SinTachieDialog.GetDialog("素材の場所にパーツが見つかりませんでした。\n画像未指定のパーツブロックを追加しますか？");
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

            //RelodeImageCommand = new ActionCommand(
            //    _ => true,
            //    _ =>
            //    {
            //        foreach (var part in Parts)
            //        {
            //            var path = part.ImagePath;
            //            part.ImagePath = path;
            //        }
            //    }
            //    );

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

        /// <summary>
        /// AddCommandで呼び出すことを想定した関数。
        /// ユーザーが指定したパーツフォルダ内に存在する最初の画像ファイルを返す。
        /// </summary>
        /// <param name="directory">ユーザが指定したフォルダのパス</param>
        /// <returns>最初の画像ファイルまたは空文字列</returns>
        private string FindFirstImageOfPart(string partName)
        {
            string directory = Path.Combine(Root, partName);
            DirectoryInfo di = new DirectoryInfo(directory);
            FileInfo[] fiAlls = di.GetFiles();
            var files = from x in fiAlls
                        where FileSettings.Default.FileExtensions.GetFileType(x.FullName) == FileType.画像
                        where !(from c in Path.GetFileName(x.FullName)
                                where c == '.'
                                select c).Skip(1).Any()
                        select x.FullName;

            if (!files.Any())
                return string.Empty;
            return files.First();
        }

        protected abstract void UpdateParts();

        protected void UpdateProperties()
        {
            UpdateParts();

            var commands = new[] { AddCommand, RemoveCommand, DuplicationCommand, MoveUpCommand, MoveDownCommand, ReloadDefaultCommand };
            foreach (var command in commands)
                command.RaiseCanExecuteChanged();
        }

        void UpdatePopupListSource()
        {
            if (string.IsNullOrEmpty(Root))
            {
                PopupListSource = [];
                return;
            }

            try
            {
                DirectoryInfo di = new(Root);
                if(FindFirstImageOfPart(string.Empty) != string.Empty)
                {
                    independentExist = true;
                }
                FileInfo[] independent = di.GetFiles();
                DirectoryInfo[] diOptions = di.GetDirectories("*", SearchOption.AllDirectories);
                List<string> newSource = independentExist ? [TagOfIndependent] : [];
                foreach (DirectoryInfo d in diOptions)
                {
                    string[] str = d.FullName.Split(Root + "\\");
                    if (str.Length != 2)
                    {
                        string clsName = GetType().Name;
                        string? mthName = MethodBase.GetCurrentMethod()?.Name;
                        SinTachieDialog.ShowError("パーツ候補の作成に失敗しました。", clsName, mthName);
                        return;
                    }
                    newSource.Add(str[1]);
                }
                PopupListSource = [.. newSource];
            }
            catch (Exception e)
            {
                SinTachieDialog.ShowWarning($"素材の場所のパスが無効です。\nエラー内容：{e.Message}");
            }
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
