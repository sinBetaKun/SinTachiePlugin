﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SinTachiePlugin.Parts;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Parts
{
    public class PartsListControllerAttribute : PropertyEditorAttribute2, IPropertyEditorForTachieParameterAttribute
    {
        public object? CharacterParameter { get; set; }

        public override FrameworkElement Create()
        {
            return new PartsListController();
        }

        public override void SetBindings(FrameworkElement control, ItemProperty[] itemProperties)
        {
            if (control is not PartsListController editor)
                return;
            var vm = new PartsListControllerViewModel(itemProperties);
            editor.DataContext = vm;
            vm.CharacterParameter = CharacterParameter as SinTachieCharacterParameter;
        }

        public override void ClearBindings(FrameworkElement control)
        {
            if (control is not PartsListController editor)
                return;
            var vm = editor.DataContext as PartsListControllerViewModel;
            vm?.Dispose();
            editor.DataContext = null;
        }
    }
}
