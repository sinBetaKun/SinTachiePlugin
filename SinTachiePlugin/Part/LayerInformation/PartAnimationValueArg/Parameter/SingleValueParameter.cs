using SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg.Argument.MultiValues;
using SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg.Argument.SingleValue;
using SinTachiePlugin.PartAnimation;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg.Parameter
{
    internal class SingleValueParameter : PartAnimationValueArgBase, ISingleValueParameter
    {
        [Display(AutoGenerateField = true)]
        public PartAnimationValue PartAnimationValue { get => _partAnimationValue; set => Set(ref  _partAnimationValue, value); }
        private PartAnimationValue _partAnimationValue = new();

        public SingleValueParameter()
        {
        }

        public SingleValueParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyFrom(PartAnimationValueArgBase? origin)
        {
            if (origin is ISingleValueParameter singleValueParameter)
                PartAnimationValue = new(singleValueParameter.PartAnimationValue);
            else if (origin is IMultiValuesParameter multiValuesParameter)
            {
                if (multiValuesParameter.PartAnimationValues.FirstOrDefault(pav => pav.Index == 0) is PartAnimationValueExtra pav)
                    PartAnimationValue = new(pav);
                else
                    PartAnimationValue = new();
            }
        }

        public override PartAnimationValueArgBase GetClone()
        {
            SingleValueParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [PartAnimationValue];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new SingleValueSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<SingleValueSharedData>() is SingleValueSharedData singleValueSharedData)
                singleValueSharedData.CopyTo(this);
        }
    }
}
