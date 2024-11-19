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
using System.Windows.Input;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Settings;

namespace SinTachiePlugin.ShapePludin.PartsListControllerForShape
{
    public partial class PartsListControllerForShapeViewModel(ItemProperty[] properties) : PartsListControllerViewModelBase(properties)
    {
        public override void SetProperties()
        {
            foreach (var property in properties)
                property.SetValue(new PartsOfShapeItem() { Root = Root, Parts = Parts });
        }

        protected override void UpdateParts()
        {
            var values = properties[0].GetValue<PartsOfShapeItem>() ?? new();
            if (!Parts.SequenceEqual(values.Parts))
            {
                Parts = [.. values.Parts];
            }
            Root = values.Root;
        }

        public override void CopyToOtherItems()
        {
            //現在のアイテムの内容を他のアイテムにコピーする
            var otherProperties = properties.Skip(1);
            for (int i = 0; i < properties.Count(); i++)
            {
                var property = properties[i];
                if (i is 0)
                {
                    property.SetValue(new PartsOfShapeItem() { Root = Root, Parts = Parts });
                }
                else
                {
                    property.SetValue(new PartsOfShapeItem
                    {
                        Parts = Parts.Select(x => new PartBlock(x)).ToImmutableList(),
                        Root = Root,
                    });
                }
            }
        }
    }
}
