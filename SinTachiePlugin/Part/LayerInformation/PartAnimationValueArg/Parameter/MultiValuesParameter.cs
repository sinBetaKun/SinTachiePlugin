using SinTachiePlugin.Control.PartAnimationValueList;
using SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg.Argment.MultiValues;
using SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg.Argment.SingleValue;
using SinTachiePlugin.PartAnimation;
using SinTachiePlugin.Properties;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg.Parameter
{
    internal class MultiValuesParameter : PartAnimationValueArgBase, IMultiValuesParameter
    {
        [Display(Name = nameof(TextResource.PartParam_LayerInfo_SourseSelectArg_PartAnimationValues), ResourceType = typeof(TextResource))]
        [PartAnimationValueList(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public ImmutableList<PartAnimationValueExtra> PartAnimationValues { get => _partAnimationValues; set => Set(ref _partAnimationValues, value); }
        private ImmutableList<PartAnimationValueExtra> _partAnimationValues = [];

        public MultiValuesParameter()
        {
        }

        public MultiValuesParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyFrom(PartAnimationValueArgBase? origin)
        {
            if (origin is IMultiValuesParameter multiValuesParameter)
                PartAnimationValues = [.. multiValuesParameter.PartAnimationValues.Select(pav => new PartAnimationValueExtra(pav))];
            else if (origin is ISingleValueParameter singleValueParameter)
                PartAnimationValues = [new(singleValueParameter.PartAnimationValue)];
        }

        public override PartAnimationValueArgBase GetClone()
        {
            MultiValuesParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [.. PartAnimationValues];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new MultiValuesSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<MultiValuesSharedData>() is MultiValuesSharedData multiValuesSharedData)
                multiValuesSharedData.CopyTo(this);
        }
    }
}