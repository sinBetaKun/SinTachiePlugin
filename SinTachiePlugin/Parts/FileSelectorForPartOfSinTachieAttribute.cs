using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Settings;
using Binding = System.Windows.Data.Binding;
using System.IO;
using YukkuriMovieMaker.Views.Converters;

namespace SinTachiePlugin.Parts
{
    public class FileSelectorForPartOfSinTachieAttribute : FileSelectorAttribute
    {
        public FileSelectorForPartOfSinTachieAttribute() : base(FileGroupType.ImageItem)
        {

        }

        public override void ClearBindings(FrameworkElement control)
        {
            BindingOperations.ClearBinding(control, FileSelector.ValueProperty);
        }

        public override FrameworkElement Create()
        {
            return new FileSelector();
        }

        public override void SetBindings(FrameworkElement control, ItemProperty[] itemProperties)
        {
            FileSelector fileSelector = (FileSelector)control;
            fileSelector.FileGroup = FileGroup;
            fileSelector.FileType = FileType;
            fileSelector.ShowThumbnail = true;
            fileSelector.ListupFilter = (x) => !(from c in Path.GetFileName(x)
                                                 where c == '.'
                                                 select c).Skip(1).Any();
            if (!string.IsNullOrEmpty(CustomFilterName) && !string.IsNullOrEmpty(CustomFilterValue))
            {
                fileSelector.Filter = CustomFilterValue;
                fileSelector.FilterName = GetCustomFilterName();
            }
            else
            {
                fileSelector.Filter = null;
                fileSelector.FilterName = null;
            }

            ItemProperty itemProperty = itemProperties[0];
            string path = (string)itemProperty.PropertyInfo.GetValue(itemProperty.PropertyOwner);
            fileSelector.DirectoryPath = Path.GetDirectoryName(path);
            ItemProperty[] properties = GetTargetProperties(itemProperties).ToArray();
            fileSelector.SetBinding(FileSelector.ValueProperty, ItemPropertiesBinding.Create(properties));
            FileSelector obj = (FileSelector)control;
        }

        private IEnumerable<ItemProperty> GetTargetProperties(ItemProperty[] itemProperties)
        {
            foreach (ItemProperty itemProperty in itemProperties)
            {
                FileSelectorAttribute customAttribute = itemProperty.PropertyInfo.GetCustomAttribute<FileSelectorAttribute>();
                if (customAttribute != null && customAttribute.FileGroup == FileGroup && customAttribute.FileType == FileType && !(customAttribute.CustomFilterName != CustomFilterName) && !(customAttribute.CustomFilterValue != CustomFilterValue))
                {
                    yield return itemProperty;
                }
            }
        }

        private string GetCustomFilterName()
        {
            if ((object)ResourceType != null)
            {
                return (string)ResourceType.GetProperty(CustomFilterName).GetValue(null);
            }

            return CustomFilterName;
        }
    }
}
