using SinTachiePlugin.Draw;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.Abrir;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.Cerrar;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.Interval;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.Offset;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.SecundaryInterval;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.Transition;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Parameter
{
    internal class RandomLoopParameter : PartAnimationOpeArgBase, IAbrirParameter, ICerrarParameter, IOffsetParameter, IIntervalParameter, ISecondaryIntervalParameter, ITransitionParameter
    {
        [Display(Name = nameof(TextResource.PartAnimationOpeArg_A), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "%", -100, 100)]
        public Animation Abrir { get; } = new Animation(100, -10000, 10000);

        [Display(Name = nameof(TextResource.PartAnimationOpeArg_B), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "%", -100, 100)]
        public Animation Cerrar { get; } = new Animation(0, -10000, 10000);

        [Display(Name = nameof(TextResource.PartAnimationOpeArg_Offset), ResourceType = typeof(TextResource))]
        [AnimationSlider("F2", nameof(TextResource.ValueUnit_Second), 0, 10, ResourceType = typeof(TextResource))]
        public Animation Offset { get; } = new Animation(0, 0, 9999);

        [Display(Name = nameof(TextResource.PartAnimationOpeArg_IntervalA), ResourceType = typeof(TextResource))]
        [AnimationSlider("F2", nameof(TextResource.ValueUnit_Second), 0, 10, ResourceType = typeof(TextResource))]
        public Animation Interval { get; } = new Animation(0, 0, 9999);

        [Display(Name = nameof(TextResource.PartAnimationOpeArg_IntervalB), ResourceType = typeof(TextResource))]
        [AnimationSlider("F2", nameof(TextResource.ValueUnit_Second), 0, 10, ResourceType = typeof(TextResource))]
        public Animation SecondaryInterval { get; } = new Animation(0, 0, 9999);

        [Display(Name = nameof(TextResource.PartAnimationOpeArg_Transition), ResourceType = typeof(TextResource))]
        [AnimationSlider("F2", nameof(TextResource.ValueUnit_Second), 0, 10, ResourceType = typeof(TextResource))]
        public Animation Transition { get; } = new Animation(0, 0, 9999);

        private readonly int _seed = Environment.TickCount;

        public override PartAnimationResult GetResult(TachieSourceDescription desc)
        {
            FrameAndLength fl = new(desc);
            int fps = desc.FPS;
            double a = fl.GetValue(Abrir, fps) / 100;
            double start = fl.GetValue(Offset, fps);
            double timespan = (double)fl.Frame / fps - start;

            if (timespan < 0)
                return PartAnimationResult.FromVolume(a);

            double transition = fl.GetValue(Transition, fps);
            double interval = fl.GetValue(Interval, fps);
            double interval2 = fl.GetValue(SecondaryInterval, fps);
            Random random = new(_seed);
            double surplus = timespan;

            while (true)
            {
                if (surplus > transition)
                {
                    double interval3 = interval + (interval2 - interval) * random.NextDouble();
                    surplus -= transition + interval3;

                    if (surplus < 0)
                        return PartAnimationResult.FromVolume(a);
                }
                else
                {
                    double rate = surplus / transition;
                    double b = fl.GetValue(Cerrar, fps) / 100;
                    return PartAnimationResult.FromVolume(a + (b - a) * rate);
                }
            }
        }

        public RandomLoopParameter()
        {
        }

        public RandomLoopParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyFrom(PartAnimationOpeArgBase? origin)
        {
            if (origin is IAbrirParameter abrirParam)
                Abrir.CopyFrom(abrirParam.Abrir);
            if (origin is ICerrarParameter cerrarParam)
                Cerrar.CopyFrom(cerrarParam.Cerrar);
            if (origin is IOffsetParameter startParam)
                Offset.CopyFrom(startParam.Offset);
            if (origin is IIntervalParameter intervalParam)
                Interval.CopyFrom(intervalParam.Interval);
            if (origin is ISecondaryIntervalParameter secondaryIntervalParam)
                Interval.CopyFrom(secondaryIntervalParam.SecondaryInterval);
            if (origin is ITransitionParameter transitionParameter)
                Transition.CopyFrom(transitionParameter.Transition);
        }

        public override PartAnimationOpeArgBase GetClone()
        {
            RandomLoopParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Abrir, Cerrar, Offset, Interval, SecondaryInterval, Transition];
        
        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new AbrirSharedData(this));
            store.Save(new CerrarSharedData(this));
            store.Save(new OffsetSharedData(this));
            store.Save(new IntervalSharedData(this));
            store.Save(new SecondaryIntervalSharedData(this));
            store.Save(new TransitionSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<AbrirSharedData>() is AbrirSharedData abrirSharedData)
                abrirSharedData.CopyTo(this);
            if (store.Load<CerrarSharedData>() is CerrarSharedData cerrarSharedData)
                cerrarSharedData.CopyTo(this);
            if (store.Load<OffsetSharedData>() is OffsetSharedData startSharedData)
                startSharedData.CopyTo(this);
            if (store.Load<IntervalSharedData>() is IntervalSharedData intervalSharedData)
                intervalSharedData.CopyTo(this);
            if (store.Load < SecondaryIntervalSharedData>() is SecondaryIntervalSharedData secondaryIntervalSharedData)
                secondaryIntervalSharedData.CopyTo(this);
            if (store.Load<TransitionSharedData>() is TransitionSharedData transitionSharedData)
                transitionSharedData.CopyTo(this);
        }
    }
}
