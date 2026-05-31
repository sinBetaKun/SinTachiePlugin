using SinTachiePlugin.Part.Origin.OriginArg.SubArgument.Argument.PointName;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.Origin.OriginArg.SubArgument.Parameter
{
    internal class OriginWithPointNameParameter : OriginSubArgBase, IPointNameParameter
    {
        [Display(Name = nameof(TextResource.PartParam_Origin_PointName), ResourceType = typeof(TextResource))]
        [TextEditor]
        public string PointName { get => _pointName; set => Set(ref _pointName, value); }
        private string _pointName = string.Empty;

        public OriginWithPointNameParameter()
        {
        }

        public OriginWithPointNameParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(OriginSubArgBase? origin)
        {
            if (origin is IPointNameParameter pointNameParameter)
                PointName = pointNameParameter.PointName;
        }

        public override OriginSubArgBase GetClone()
        {
            OriginWithPointNameParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new PointNameSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<PointNameSharedData>() is PointNameSharedData pointNameSharedData)
                PointName = pointNameSharedData.PointName;
        }
    }
}
