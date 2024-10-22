using Newtonsoft.Json;
using SinTachiePlugin.Informations;
using SinTachiePlugin.Parts;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Settings;

namespace SinTachiePlugin.Parts
{
    public class PartsListControllerViewModel : Bindable, INotifyPropertyChanged, IPropertyEditorControl, IDisposable
    {
        readonly INotifyPropertyChanged item;
        readonly ItemProperty[] properties;

        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public ImmutableList<PartBlock> Parts { get => parts;  set => Set(ref parts, value); }
        ImmutableList<PartBlock> parts = [];

        public int SelectedIndex { get => selectedIndex; set => Set(ref selectedIndex, value); }
        int selectedIndex = -1;
        public SinTachieCharacterParameter? CharacterParameter
        {
            get => characterParameter;
            set 
            {
                var oldCP = characterParameter;
                if (Set(ref characterParameter, value))
                {
                    if (oldCP != null)
                    {
                        oldCP.PropertyChanged -= CharacterParameterChanged;
                    }
                    if (characterParameter != null)
                    {
                        characterParameter.PropertyChanged += CharacterParameterChanged;
                    }
                    setRoot();
                }
            }
        }
        SinTachieCharacterParameter? characterParameter;
        private void CharacterParameterChanged(object sender, PropertyChangedEventArgs e)
        {
            setRoot();
        }
        private void setRoot()
        {
            Root = characterParameter?.Directory ?? string.Empty;
        }

        public string Root { get => root; set { Set(ref root, value); updatePartsComboSource(); } }
        string root = string.Empty;

        public string[] PartsComboSource { get => partsComboSource; set => Set(ref partsComboSource, value); }
        string[] partsComboSource = new string[0];

        public string SelectedAddingPart { get => selectedAddingPart; set => Set(ref selectedAddingPart, value); }
        string selectedAddingPart = string.Empty;

        public ActionCommand AddCommand { get; }
        public ActionCommand RemoveCommand { get; }
        public ActionCommand DuplicationCommand { get; }
        public ActionCommand MoveUpCommand { get; }
        public ActionCommand MoveDownCommand { get; }
        public ActionCommand RelodeImageCommand { get; }
        public ActionCommand WriteDefaultCommand { get; }
        public ActionCommand DeleteDefaultCommand { get; }
        public ActionCommand ReloadDefaultCommand { get; }

        public PartsListControllerViewModel(ItemProperty[] properties)
        {
            this.properties = properties;

            item = (INotifyPropertyChanged)properties[0].PropertyOwner;
            item.PropertyChanged += Item_PropertyChanged;

            AddCommand = new ActionCommand(
                _ => true,
                _ =>
                {
                    if (String.IsNullOrWhiteSpace(selectedAddingPart))
                    {
                        SinTachieDialog.ShowWarning("「追加するパーツ」が選択されていないため、パーツを追加できません。");
                        return;
                    }
                    var tmpSelectedIndex = SelectedIndex;
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    string partImagePath = FindFirstImageOfPart(SelectedAddingPart);
                    string tag = SelectedAddingPart;
                    var tags = from part in Parts select part.TagName;
                    if (tags.Contains(tag))
                    {
                        int sideNum = 1;
                        while (tags.Contains($"{tag}({sideNum})")) sideNum++;
                        tag += $"({sideNum})";
                    }
                    Parts = Parts.Insert(tmpSelectedIndex + 1, new PartBlock(partImagePath, tag));
                    foreach (var property in properties)
                        property.SetValue(Parts);
                    EndEdit?.Invoke(this, EventArgs.Empty);
                    SelectedIndex = tmpSelectedIndex + 1;
                });

            RemoveCommand = new ActionCommand(
                _ => parts.Count > 0 && SelectedIndex > -1,
                _ =>
                {
                    var tmpSelectedIndex = SelectedIndex;
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    Parts = Parts.RemoveAt(tmpSelectedIndex);
                    foreach (var property in properties)
                        property.SetValue(Parts);
                    EndEdit?.Invoke(this, EventArgs.Empty);
                    if (parts.Count > 0) SelectedIndex = Math.Min(tmpSelectedIndex, parts.Count - 1);
                });

            DuplicationCommand = new ActionCommand(
                _ => SelectedIndex > -1,
                _ =>
                {
                    var tmpSelectedIndex = SelectedIndex;
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    var copied = new PartBlock(Parts[SelectedIndex]/*, ImagePathCopyMode.BySetter*/);
                    Parts = Parts.Insert(tmpSelectedIndex + 1, copied);
                    foreach (var property in properties)
                        property.SetValue(Parts);
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
                    foreach (var property in properties)
                        property.SetValue(Parts);
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
                    foreach (var property in properties)
                        property.SetValue(Parts);
                    EndEdit?.Invoke(this, EventArgs.Empty);
                    SelectedIndex = tmpSelectedIndex + 1;
                });

            RelodeImageCommand = new ActionCommand(
                _ => true,
                _ =>
                {
                    foreach (var part in Parts)
                    {
                        var path = part.ImagePath;
                        part.ImagePath = path;
                    }
                }
                );

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
                    selected.ReloadDefault();
                    EndEdit?.Invoke(this, EventArgs.Empty);
                });

            UpdateParts();
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == properties[0].PropertyInfo.Name)
                UpdateParts();
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

            if (files.Count() < 1)
                return string.Empty;
            return files.First();
        }

        void UpdateParts()
        {
            var values = properties[0].GetValue<ImmutableList<PartBlock>>() ?? [];
            if (!Parts.SequenceEqual(values))
            {
                Parts = [.. values];
            }

            var commands = new[] { AddCommand, RemoveCommand, DuplicationCommand, MoveUpCommand, MoveDownCommand, ReloadDefaultCommand};
            foreach (var command in commands)
                command.RaiseCanExecuteChanged();
        }

        void updatePartsComboSource()
        {
            if (string.IsNullOrEmpty(Root))
            {
                PartsComboSource = [];
                return;
            }

            try
            {
                DirectoryInfo di = new DirectoryInfo(Root);
                DirectoryInfo[] diOptions = di.GetDirectories("*", SearchOption.AllDirectories);
                List<string> newSource = new List<string>();
                foreach (DirectoryInfo d in diOptions)
                {
                    string[] str = d.FullName.Split(Root + "\\");
                    if(str.Length != 2)
                    {
                        string clsName = GetType().Name;
                        string? mthName = MethodBase.GetCurrentMethod()?.Name;
                        SinTachieDialog.ShowError("パーツ候補の作成に失敗しました。", clsName, mthName);
                        return;
                    }
                    newSource.Add(str[1]);
                }
                PartsComboSource = newSource.ToArray();
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
