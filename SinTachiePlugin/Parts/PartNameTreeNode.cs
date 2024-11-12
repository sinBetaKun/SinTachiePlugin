using SinTachiePlugin.Informations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Settings;

namespace SinTachiePlugin.Parts
{
    public class PartNameTreeNode
    {
        public string Name { get; set; }

        public string FullName { get; set; }

        public IEnumerable<PartNameTreeNode> Children { get; set; } = [];

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
                        Children = Children.Append(preChild);
                    }
                }
                var fis = from x in di.GetFiles()
                          where FileSettings.Default.FileExtensions.GetFileType(x.FullName) == FileType.画像
                          where !(from c in Path.GetFileName(x.FullName)
                                  where c == '.'
                                  select c).Skip(1).Any()
                          select x;
                Children = Children.Concat(fis.Select(fi => new PartNameTreeNode(fi)));

                if (Children.Any())
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
