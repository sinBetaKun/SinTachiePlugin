﻿using SinTachiePlugin.Informations;
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
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Settings;

namespace SinTachiePlugin.ShapePludin.PartsListControllerForShape
{
    public class PartsListControllerForShapeViewModel(ItemProperty[] properties) : PartsListControllerViewModelBase(properties)
    {
        protected override void SetProparties()
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
    }
}
