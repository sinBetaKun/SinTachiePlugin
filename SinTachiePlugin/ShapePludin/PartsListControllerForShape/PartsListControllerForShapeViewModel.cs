﻿using SinTachiePlugin.Parts;
using System.Collections.Immutable;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.ShapePludin.PartsListControllerForShape
{
    public class PartsListControllerForShapeViewModel(ItemProperty[] properties) : PartsListControllerViewModelBase(properties)
    {
        public override void SetProperties()
        {
            for (int i = 0; i < properties.Count(); i++)
            {
                var property = properties[i];
                property.SetValue(new PartsOfShapeItem
                {
                    Parts = Parts.Select(x => new PartBlock(x)).ToImmutableList(),
                    Root = Root,
                });
            }
        }

        protected override void UpdateParts()
        {
            var values = properties[0].GetValue<PartsOfShapeItem>() ?? new();
            if (!Parts.SequenceEqual(values.Parts))
            {
                Parts = [.. values.Parts];
                Parts.ForEach(x => x.Selected = false);
            }
            Root = values.Root;
        }

        public override void CopyToOtherItems()
        {
            //現在のアイテムの内容を他のアイテムにコピーする
            var otherProperties = properties.Skip(1);
            for (int i = 0; i < otherProperties.Count(); i++)
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
