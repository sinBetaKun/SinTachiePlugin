using SinTachiePlugin.Control.CustomNamePointList;
using SinTachiePlugin.Part.CustomPoint.CustomPointArg.Argment.Points;
using System.Collections.Immutable;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.CustomPoint.CustomPointArg.Parameter
{
    internal class HavingSourceCustomPointParameter : CustomPointArgBase, IPointsParameter
    {
        [CustomNamePointList(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public ImmutableList<CustomNamePoint> Points { get => _points; set => Set(ref _points, value); }
        private ImmutableList<CustomNamePoint> _points = [];

        public HavingSourceCustomPointParameter()
        {
        }

        public HavingSourceCustomPointParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(CustomPointArgBase? origin)
        {
            if (origin is IPointsParameter pointsParameter)
                Points = [.. pointsParameter.Points];
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new PointsSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<PointsSharedData>() is PointsSharedData pointsSharedData)
                Points = [.. pointsSharedData.Points];
        }
    }
}