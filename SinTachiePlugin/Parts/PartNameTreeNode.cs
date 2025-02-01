using System.IO;
using YukkuriMovieMaker.Settings;

namespace SinTachiePlugin.Parts
{
    public class PartNameTreeNode
    {
        public string Name { get; set; }

        public string FullName { get; set; }

        public List<PartNameTreeNode> Children { get; set; } = [];

        public PartNameTreeNode()
        {
            FullName = string.Empty;
            Name = string.Empty;
        }

        public PartNameTreeNode(FileInfo fi)
        {
            Name = fi.Name;
            FullName = fi.FullName;
        }

        public PartNameTreeNode(DirectoryInfo di)
        {
            try
            {
                DirectoryInfo[] subdis = di.GetDirectories();
                foreach (DirectoryInfo subdi in subdis)
                {
                    PartNameTreeNode preChild = new(subdi);
                    if (!string.IsNullOrEmpty(preChild.Name))
                    {
                        Children.Add(preChild);
                    }
                }
                var fis = (from x in di.GetFiles()
                           where FileSettings.Default.FileExtensions.GetFileType(x.FullName) == FileType.画像
                           where !(from c in Path.GetFileName(x.FullName)
                                   where c == '.'
                                   select c).Skip(1).Any()
                           select new PartNameTreeNode(x)).ToList();
                Children.AddRange(fis);

                if (!Children.Any())
                {
                    FullName = string.Empty;
                    Name = string.Empty;
                    return;
                }

                FullName = di.FullName;
                Name = di.Name;
            }
            catch (Exception)
            {
                FullName = string.Empty;
                Name = string.Empty;
            }
        }
    }
}
