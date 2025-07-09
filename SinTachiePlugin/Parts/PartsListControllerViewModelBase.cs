using Newtonsoft.Json;
using SinTachiePlugin.Informations;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Settings;

namespace SinTachiePlugin.Parts
{
    public abstract class PartsListControllerViewModelBase : Bindable, INotifyPropertyChanged, IPropertyEditorControl2, IDisposable
    {
        readonly INotifyPropertyChanged item;
        protected readonly ItemProperty[] properties;
        static string clipedBlocks = "";

        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public int ListHeight
        {
            get => PluginInfo.PartsListHeight;
            set => Set(ref PluginInfo.PartsListHeight, value);
        }

        /// <summary>
        /// パーツブロックのリストの内容
        /// </summary>
        public ImmutableList<PartBlock> Parts { get => parts; set => Set(ref parts, value); }
        ImmutableList<PartBlock> parts = [];

        /// <summary>
        /// リストで選択されているパーツブロックのインデックス
        /// </summary>
        public int SelectedPartIndex { get => selectedPartIndex; set => Set(ref selectedPartIndex, value); }
        int selectedPartIndex = -1;

        /// <summary>
        /// リスト内のいずれかのパーツブロックが選択されている状態か否か
        /// </summary>
        public bool SomeBlockSelected { get => someBlockSelected; set => Set(ref someBlockSelected, value); }
        bool someBlockSelected = false;

        /// <summary>
        /// リスト内のパーツブロックが一つ以下選択されている状態か否か
        /// </summary>
        public bool UnMultiBlockSelected { get => unMultiBlockSelected; set => Set(ref unMultiBlockSelected, value); }
        bool unMultiBlockSelected = true;

        /// <summary>
        /// リスト内のパーツブロックが一つだけ選択されている状態か否か
        /// </summary>
        public bool SingleBlockSelected { get => singleBlockSelected; set => Set(ref singleBlockSelected, value); }
        bool singleBlockSelected = false;

        /// <summary>
        /// 選択中のパーツブロックを上に移動できるか。
        /// </summary>
        public bool CanMoveUp { get => canMoveUp; set => Set(ref canMoveUp, value); }
        bool canMoveUp = false;

        /// <summary>
        /// 選択中のパーツブロックを下に移動できるか。
        /// </summary>
        public bool CanMoveDown { get => canMoveDown; set => Set(ref canMoveDown, value); }
        bool canMoveDown = false;

        /// <summary>
        /// 選択されているブロックの組み合わせが変化すると呼び出される
        /// </summary>
        /// <param name="selecteds"></param>
        public void UpdateButtonEnables(IEnumerable<PartBlock> selecteds)
        {
            Parts.ForEach(part => part.Selected = false);
            if (!selecteds.Any())
            {
                SomeBlockSelected = CanMoveUp = CanMoveDown = SingleBlockSelected = false;
                UnMultiBlockSelected = true;
                return;
            }
            
            SomeBlockSelected = true;
            UnMultiBlockSelected = SingleBlockSelected = selecteds.Count() < 2;
            int[] indexs = [.. selecteds.Select(i => Parts.IndexOf(i)).OrderBy(i => i)];
            CanMoveUp = indexs.Min() > 0;
            CanMoveDown = indexs.Max() < Parts.Count - 1;

            foreach (var part in selecteds)
            {
                part.Selected = true;
            }
        }

        readonly List<(TreeViewItem, string)> nameTreeItems = [];

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
                    nodes.Add(t);
                }
            }

            IEnumerable<TreeViewItem> leafs = target.GetFiles()
                .Where(fi => FileSettings.Default.FileExtensions.GetFileType(fi.FullName) == FileType.画像)
                .Where(fi=> !(from c in fi.Name
                              where c == '.'
                              select c).Skip(1).Any())
                .Select(fi =>
                {
                    TreeViewItem t = new() { Header = fi.Name };
                    t.MouseLeftButtonUp += PartTreeNodeClick;
                    nameTreeItems.Add((t, fi.FullName));
                    return t;
                });
            return [.. nodes, .. leafs];
        }

        private void PartTreeNodeClick(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem item)
            {
                string tagName;
                string partImagePath = nameTreeItems.Find(t => t.Item1 == item).Item2;
                string fromRoot = partImagePath.Split(Root + "\\").Last();
                if (Path.GetDirectoryName(fromRoot) is string dn && !string.IsNullOrEmpty(dn))
                {
                    tagName = dn;
                }
                else
                {
                    tagName = Path.GetFileNameWithoutExtension((string)item.Header);
                }

                string[] tags = [.. from part in Parts select part.TagName];
                if (tags.Contains(tagName))
                {
                    int sideNum = 1;
                    while (tags.Contains($"{tagName}({sideNum})")) sideNum++;
                    tagName += $"({sideNum})";
                }
                int tmpSelectedIndex;
                BeginEdit?.Invoke(this, EventArgs.Empty);
                if (SelectedPartIndex < 0)
                {
                    tmpSelectedIndex = Parts.Count;
                    Parts = Parts.Add(new PartBlock(partImagePath, tagName, tags));
                    SetProperties();
                }
                else
                {
                    tmpSelectedIndex = SelectedPartIndex;
                    Parts = Parts.Insert(tmpSelectedIndex, new PartBlock(partImagePath, tagName, tags));
                    SetProperties();
                }
                EndEdit?.Invoke(this, EventArgs.Empty);
                SelectedPartIndex = tmpSelectedIndex;
                PartsPopupIsOpen = false;
            }
        }

        public void AddPart(System.Windows.Controls.TreeView treeView)
        {
            treeView.Items.Clear();
            bool rootUnexist = !Path.Exists(Root);
            if (!rootUnexist)
            {
                try
                {
                    DirectoryInfo di = new(Root);
                    foreach (TreeViewItem item in CreateTreeItem(di))
                    {
                        treeView.Items.Add(item);
                    }
                }
                catch (Exception e)
                {
                    SinTachieDialog.ShowError(e);
                }
            }

            if (treeView.Items.Count == 0 || rootUnexist)
            {
                var intro = rootUnexist ? "素材の場所のパスが無効です。" : "素材の場所にパーツが見つかりませんでした。";
                var dialog = SinTachieDialog.GetOKorCancel($"{intro}\n画像未指定のパーツブロックを追加しますか？");
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
        }

        /// <summary>
        /// 右クリックメニューで「切り取り」を選択したときの処理
        /// </summary>
        public void CutFunc(IEnumerable<PartBlock> selecteds)
        {
            BeginEdit?.Invoke(this, EventArgs.Empty);
            CopyFunc(selecteds);
            RemovePartBlock(selecteds);
            EndEdit?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 右クリックメニューで「コピー」を選択したときの処理
        /// </summary>
        public void CopyFunc(IEnumerable<PartBlock> selecteds)
        {
            clipedBlocks = ConvertBlocks2Json(selecteds.OrderBy(Parts.IndexOf));
        }

        /// <summary>
        /// 右クリックメニューで「貼り付け」が選択可能か否か
        /// </summary>
        public bool PasteEnable { get => pasteEnable; set => Set(ref pasteEnable, value); }
        bool pasteEnable = false;

        string GetNewTag(PartBlock block)
        {
            string ret = block.TagName;
            int count = 0;

            while (true)
            {
                bool ok = true;
                foreach (var part in parts)
                {
                    if (part.TagName == ret)
                    {
                        ok = false;
                        break;
                    }
                }
                
                if (ok)
                {
                    if (count != 0)
                    {
                        if (SinTachieDialog.GetYESorNO(Resources.SameTagWarning + block.TagName) == DialogResult.Yes)
                        {
                            return ret;
                        }
                    }

                    return block.TagName;
                }

                ret = block.TagName + $"({++count})";
            }
        }

        string GetNewTagD(PartBlock block)
        {
            string ret = block.TagName + "(1)";
            int count = 1;

            while (true)
            {
                bool ok = true;
                foreach (var part in parts)
                {
                    if (part.TagName == ret)
                    {
                        ok = false;
                        break;
                    }
                }
                
                if (ok)
                {
                    return ret;
                }

                ret = block.TagName + $"({++count})";
            }
        }

        /// <summary>
        /// 右クリックメニューで「貼り付け」を選択したときの処理
        /// </summary>
        public IList<PartBlock>? PasteFunc()
        {
            if (clipedBlocks == null)
            {
                SinTachieDialog.ShowError(new("疑似クリップボードにnullが代入されている状態での貼り付け処理は、本来なら不可能な処理です。"));
                return null;
            }

            if (GetBlocksFromJson(clipedBlocks) is PartBlock[] parts)
            {
                var tmpSelectedIndex = SelectedPartIndex;
                BeginEdit?.Invoke(this, EventArgs.Empty);

                if (tmpSelectedIndex < 0)
                {
                    foreach (PartBlock part in parts)
                    {
                        part.TagName = GetNewTag(part);
                        Parts = Parts.Add(part);
                    }

                    tmpSelectedIndex = Parts.Count - 1;
                }
                else
                {
                    int index = tmpSelectedIndex;

                    foreach (PartBlock part in parts)
                    {
                        part.TagName = GetNewTag(part);
                        Parts = Parts.Insert(index++, part);
                    }
                }
                SetProperties();
                EndEdit?.Invoke(this, EventArgs.Empty);
                SelectedPartIndex = tmpSelectedIndex;
                return parts;
            }
            return null;
        }

        /// <summary>
        /// 右クリックメニューで「複製」を選択したときの処理
        /// </summary>
        public IList<PartBlock> DuplicationFunc(IEnumerable<PartBlock> selecteds)
        {
            var tmpSelectedIndex = SelectedPartIndex;
            var jsonOfSelecteds = ConvertBlocks2Json(selecteds.OrderBy(Parts.IndexOf));
            if (GetBlocksFromJson(jsonOfSelecteds) is PartBlock[] tmp)
            {
                int[] indexs = [.. selecteds.Select(i => Parts.IndexOf(i)).OrderBy(i => i)];
                List<PartBlock> parts = [.. Parts];
                int shift = 1;
                bool denySameTag = SinTachieDialog.GetYESorNO(Resources.DuplicateWarning) == DialogResult.Yes;

                if (denySameTag)
                {
                    for (int i = 0; i < tmp.Length - 1; i++)
                    {
                        tmp[i].TagName = GetNewTagD(tmp[i]);
                        parts.Insert(indexs[i] + shift++, tmp[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < tmp.Length - 1; i++)
                    {
                        parts.Insert(indexs[i] + shift++, tmp[i]);
                    }
                }

                int lastIndex = indexs.Last();
                PartBlock lastBlock = tmp.Last();

                if (denySameTag)
                {
                    lastBlock.TagName = GetNewTagD(lastBlock);
                }

                if (lastIndex + 1 < Parts.Count)
                {
                    parts.Insert(lastIndex + shift, lastBlock);
                }
                else
                {
                    parts.Add(lastBlock);
                }

                BeginEdit?.Invoke(this, EventArgs.Empty);
                Parts = [.. parts];
                SetProperties();
                EndEdit?.Invoke(this, EventArgs.Empty);
                SelectedPartIndex = tmpSelectedIndex + 1;
                return tmp;
            }

            return [.. selecteds];
        }

        /// <summary>
        /// 右クリックメニューで「削除」を選択したときの処理
        /// </summary>
        public void RemoveFunc(IEnumerable<PartBlock> selecteds)
        {
            BeginEdit?.Invoke(this, EventArgs.Empty);
            RemovePartBlock(selecteds);
            EndEdit?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 右クリックメニューで「全ブロックにチェック」を選択したときの処理
        /// </summary>
        public void CheckAll()
        {
            BeginEdit?.Invoke(this, EventArgs.Empty);
            Parts.ForEach(i => i.Appear = true);
            SetProperties();
            EndEdit?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 右クリックメニューで「選択中のブロックにのみチェック」を選択したときの処理
        /// </summary>
        public void CheckOnlySelected(IEnumerable<PartBlock> selecteds)
        {
            BeginEdit?.Invoke(this, EventArgs.Empty);
            Parts.ForEach(i => i.Appear = false);
            foreach (var selected in selecteds) selected.Appear = true;
            SetProperties();
            EndEdit?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 選択されているブロックを一つ上に動かす。
        /// </summary>
        public void MoveUpSelected(IEnumerable<PartBlock> selecteds)
        {
            var tmpSelectedIndex = SelectedPartIndex;
            List<PartBlock> parts = [.. Parts];
            PartBlock[] tmp = [.. selecteds.OrderBy(Parts.IndexOf)];

            for (int i = 0; i < tmp.Length; i++)
            {
                int index = parts.IndexOf(tmp[i]);
                parts.Remove(tmp[i]);
                parts.Insert(index - 1, tmp[i]);
            }

            BeginEdit?.Invoke(this, EventArgs.Empty);
            Parts = [.. parts];
            SetProperties();
            EndEdit?.Invoke(this, EventArgs.Empty);
            SelectedPartIndex = tmpSelectedIndex - 1;
        }


        /// <summary>
        /// 選択されているブロックを一つ上に動かす。
        /// </summary>
        public void MoveDownSelected(IEnumerable<PartBlock> selecteds)
        {
            var tmpSelectedIndex = SelectedPartIndex;
            BeginEdit?.Invoke(this, EventArgs.Empty);
            List<PartBlock> parts = [.. Parts];
            PartBlock[] tmp = [.. selecteds.OrderBy(part => -Parts.IndexOf(part))];

            for (int i = 0; i < tmp.Length - 1; i++)
            {
                int index = parts.IndexOf(tmp[i]);
                parts.Remove(tmp[i]);
                parts.Insert(index + 1, tmp[i]);
            }

            int index2 = parts.IndexOf(tmp.Last());
            parts.Remove(tmp.Last());

            if (index2 < parts.Count)
            {
                parts.Insert(index2 + 1, tmp.Last());
            }
            else
            {
                parts.Add(tmp.Last());
            }
                
            Parts = [.. parts];
            SetProperties();
            EndEdit?.Invoke(this, EventArgs.Empty);
            SelectedPartIndex = tmpSelectedIndex + 1;
        }



        /// <summary>
        /// デフォルト設定のリロードを選択したときの処理
        /// </summary>
        public void ReloadFunc(IEnumerable<PartBlock> selecteds)
        {
            var tmpSelectedPartIndex = SelectedPartIndex;
            BeginEdit?.Invoke(this, EventArgs.Empty);
            foreach (var selected in selecteds)
            {
                selected.ReloadDefault();
            }
            SetProperties();
            EndEdit?.Invoke(this, EventArgs.Empty);
            SelectedPartIndex = tmpSelectedPartIndex;
        }

        /// <summary>
        /// JSONデータからPartBlock[]を取得するメソッド
        /// </summary>
        /// <returns></returns>
        private PartBlock[]? GetBlocksFromJson(string json)
        {
            if (JsonConvert.DeserializeObject<PartBlock[]>(json, PartBlock.GetJsonSetting) is PartBlock[] parts)
            {
                return parts;
            }
            else
            {
                SinTachieDialog.ShowError(new("JSONデータからパーツブロック配列を取得できませんでした。"));
                return null;
            }
        }

        private static string ConvertBlocks2Json(IEnumerable<PartBlock> blocks) =>
            JsonConvert.SerializeObject(blocks, Formatting.Indented, PartBlock.GetJsonSetting);

        /// <summary>
        /// 右クリックメニューが開かれている状態か否か
        /// </summary>
        public bool ContextMenuIsOpen
        {
            get => contextMeneIsOpen;
            set
            {
                PasteEnable = JsonConvert.DeserializeObject<PartBlock[]>(clipedBlocks, PartBlock.GetJsonSetting) is not null;
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
        //public List<PartNameTreeNode> PartNameTree { get => partNameTreeNode; set => Set(ref partNameTreeNode, value); }
        //List<PartNameTreeNode> partNameTreeNode = [];

        /// <summary>
        /// パーツ選択ツリーが表示されているか否か
        /// </summary>
        public bool PartsPopupIsOpen
        {
            get => partsPopupIsOpen;
            set {
                if (!value) nameTreeItems.Clear();
                Set(ref partsPopupIsOpen, value);
            }
        }
        bool partsPopupIsOpen = false;

        public ActionCommand WriteDefaultCommand { get; }
        public ActionCommand DeleteDefaultCommand { get; }

        public PartsListControllerViewModelBase(ItemProperty[] properties)
        {
            this.properties = properties;

            item = (INotifyPropertyChanged)properties[0].PropertyOwner;
            item.PropertyChanged += Item_PropertyChanged;

            WriteDefaultCommand = new ActionCommand(
                _ => SingleBlockSelected,
                _ =>
                {
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    var selected = Parts[SelectedPartIndex];
                    if (selected == null)
                    {
                        SinTachieDialog.ShowError(new("選択されたブロックを取得できません。"));
                        return;
                    }
                    selected.UpdateDefault();
                    EndEdit?.Invoke(this, EventArgs.Empty);
                });

            DeleteDefaultCommand = new ActionCommand(
                _ => SingleBlockSelected,
                _ =>
                {
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    var selected = Parts[SelectedPartIndex];
                    if (selected == null)
                    {
                        SinTachieDialog.ShowError(new("選択されたブロックを取得できません。"));
                        return;
                    }
                    selected.DeleteDafault();
                    EndEdit?.Invoke(this, EventArgs.Empty);
                });

            UpdateProperties();
        }

        private void RemovePartBlock(IEnumerable<PartBlock> selecteds)
        {
            var tmpSelectedIndex = SelectedPartIndex;
            Parts = Parts.RemoveRange(selecteds);
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

            // ボタンの有効・無効を状況によって切り替える必要のあるコマンド
            var commands = new[] { WriteDefaultCommand, DeleteDefaultCommand };
            foreach (var command in commands)
                command.RaiseCanExecuteChanged();
        }

        public abstract void CopyToOtherItems();

        public void Dispose()
        {
            item.PropertyChanged -= Item_PropertyChanged;
        }

        public void SetEditorInfo(IEditorInfo info)
        {
        }
    }
}
