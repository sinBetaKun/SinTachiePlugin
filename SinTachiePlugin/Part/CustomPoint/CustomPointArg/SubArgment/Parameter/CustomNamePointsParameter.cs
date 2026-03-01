using SinTachiePlugin.Control.CustomNamePointList;
using SinTachiePlugin.Part.CustomPoint.CustomPointArg.Argment.SubArgment.Points;
using System.Collections.Immutable;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgment.Parameter
{
    internal class CustomNamePointsParameter : CustomPointSubArgBase, IPointsParameter
    {
        [CustomNamePointList(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public ImmutableList<CustomNamePoint> Points { get => _points; set => Set(ref _points, value); }
        private ImmutableList<CustomNamePoint> _points = [];

        public CustomNamePointsParameter()
        {
        }

        public CustomNamePointsParameter(SharedDataStore? store = null) : base(store)
        {   
        }

        public override void CopyFrom(CustomPointSubArgBase? origin)
        {
            if (origin is IPointsParameter pointsParameter)
                Points = [.. pointsParameter.Points.Select(p => new CustomNamePoint(p))];
        }

        public override CustomPointSubArgBase GetClone()
        {
            CustomNamePointsParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new PointsSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<PointsSharedData>() is PointsSharedData pointsSharedData)
                Points = [.. pointsSharedData.Points.Select(p => new CustomNamePoint(p))];
        }
    }
}
