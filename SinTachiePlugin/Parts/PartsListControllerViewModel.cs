﻿using Newtonsoft.Json;
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
    public class PartsListControllerViewModel(ItemProperty[] properties) : PartsListControllerViewModelBase(properties)
    {
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
                    SetRoot();
                }
            }
        }
        SinTachieCharacterParameter? characterParameter;

        protected override void SetProparties()
        {
            foreach (var property in properties)
                property.SetValue(Parts);
        }

        private void CharacterParameterChanged(object sender, PropertyChangedEventArgs e)
        {
            SetRoot();
        }

        private void SetRoot()
        {
            Root = characterParameter?.Directory ?? string.Empty;
        }

        protected override void UpdateParts()
        {
            var values = properties[0].GetValue<ImmutableList<PartBlock>>() ?? [];
            if (!Parts.SequenceEqual(values))
            {
                Parts = [.. values];
            }
        }
    }
}
