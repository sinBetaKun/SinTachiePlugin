using System.ComponentModel.DataAnnotations;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.Abrir;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.Cerrar;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.Interval;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.Start;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.Transition;
using SinTachiePlugin.Parts;
using SinTachiePlugin.Properties;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Parameter
{
    internal class PeriodicShuttleParameter : PartAnimationOpeArgBase, IAbrirParameter, ICerrarParameter, IStartParameter, IIntervalParameter, ITransitionParameter
    {
        [Display(Name = nameof(Text.PartAnimationOpeArg_A), ResourceType = typeof(Text))]
        [AnimationSlider("F1", "%", -100, 100)]
        public Animation Abrir { get; } = new Animation(100, -10000, 10000);

        [Display(Name = nameof(Text.PartAnimationOpeArg_B), ResourceType = typeof(Text))]
        [AnimationSlider("F1", "%", -100, 100)]
        public Animation Cerrar { get; } = new Animation(0, -10000, 10000);

        [Display(Name = nameof(Text.PartAnimationOpeArg_Start), ResourceType = typeof(Text))]
        [AnimationSlider("F2", nameof(Text.ValueUnit_Second), 0, 10, ResourceType = typeof(Text))]
        public Animation Start { get; } = new Animation(0, 0, 9999);

        [Display(Name = nameof(Text.PartAnimationOpeArg_Interval), ResourceType = typeof(Text))]
        [AnimationSlider("F2", nameof(Text.ValueUnit_Second), 0, 10, ResourceType = typeof(Text))]
        public Animation Interval { get; } = new Animation(0, 0, 9999);

        [Display(Name = nameof(Text.PartAnimationOpeArg_Transition), ResourceType = typeof(Text))]
        [AnimationSlider("F2", nameof(Text.ValueUnit_Second), 0, 10, ResourceType = typeof(Text))]
        public Animation Transition { get; } = new Animation(0, 0, 9999);

        /// <summary>
        /// 制御モードが周期的往復/ループのとき、差分を指定する値を返す。
        /// </summary>
        /// <param name="fl">アイテムのフレームと長さ</param>
        /// <param name="fps">fps</param>
        /// <returns>出力</returns>
        public override double GetValue(FrameAndLength fl, int fps, double voiceVolume)
        {
            double a = fl.GetValue(Abrir, fps) / 100;
            double start = fl.GetValue(Start, fps);
            double timespan = (double)fl.Frame / fps - start;

            if (timespan < 0)
                return a;

            double interval = fl.GetValue(Interval, fps);
            double transition = fl.GetValue(Transition, fps);
            double surplus = timespan % (transition + interval);

            if (surplus > transition)
                return a;

            double rate = surplus / transition;
            double b = fl.GetValue(Cerrar, fps) / 100;

            return b + (a - b) * (Math.Abs(rate - 0.5) * 2);
        }

        public PeriodicShuttleParameter()
        {
        }

        public PeriodicShuttleParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyFrom(PartAnimationOpeArgBase? origin)
        {
            if (origin is IAbrirParameter abrirParam)
                Abrir.CopyFrom(abrirParam.Abrir);
            if (origin is ICerrarParameter cerrarParam)
                Cerrar.CopyFrom(cerrarParam.Cerrar);
            if (origin is IStartParameter startParam)
                Start.CopyFrom(startParam.Start);
            if (origin is IIntervalParameter intervalParam)
                Interval.CopyFrom(intervalParam.Interval);
            if (origin is ITransitionParameter transitionParameter)
                Transition.CopyFrom(transitionParameter.Transition);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Abrir, Cerrar];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new AbrirSharedData(this));
            store.Save(new CerrarSharedData(this));
            store.Save(new StartSharedData(this));
            store.Save(new IntervalSharedData(this));
            store.Save(new TransitionSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<AbrirSharedData>() is AbrirSharedData abrirParameter)
                abrirParameter.CopyTo(this);
            if (store.Load<CerrarSharedData>() is CerrarSharedData cerrarParameter)
                cerrarParameter.CopyTo(this);
            if (store.Load<StartSharedData>() is StartSharedData startSharedData)
                startSharedData.CopyTo(this);
            if (store.Load<IntervalSharedData>() is IntervalSharedData intervalSharedData)
                intervalSharedData.CopyTo(this);
            if (store.Load<TransitionSharedData>() is TransitionSharedData transitionSharedData)
                transitionSharedData.CopyTo(this);
        }
    }
}