using System.Collections.Immutable;
using System.ComponentModel;
using YukkuriMovieMaker.Commons;

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
      
        public override void SetProperties()
        {
            foreach (var property in properties)
                property.SetValue(Parts.Select(x => new PartBlock(x)).ToImmutableList());
        }

        public override void CopyToOtherItems()
        {
            //現在のアイテムの内容を他のアイテムにコピーする
            var otherProperties = properties.Skip(1);
            foreach (var property in otherProperties)
                property.SetValue(Parts.Select(x => new PartBlock(x)).ToImmutableList());
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
                Parts.ForEach(x => x.Selected = false);
            }
        }
    }
}
