using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.LayerValueListController.Extra.Parameter
{
    public class PeriodicParameter : LayerValueExtraBase, IStartParameter, IIntervalParameter, ITransitionParameter
    {
        [Display(Name = "開始", Description = "差分を動かし始めるタイミング")]
        [AnimationSlider("F2", "秒", 0, 10)]
        public Animation Start { get; } = new Animation(0, 0, 9999);

        [Display(Name = "間隔", Description = "差分を動かす時間間隔（遷移は含まれない）")]
        [AnimationSlider("F2", "秒", 0, 10)]
        public Animation Interval { get; } = new Animation(0, 0, 9999);

        [Display(Name = "遷移", Description = "差分を動かし始めてから終わるまでの時間")]
        [AnimationSlider("F2", "秒", 0, 10)]
        public Animation Transition { get; } = new Animation(0, 0, 9999);

        /// <summary>
        /// 制御モードが周期的往復/ループのとき、差分を指定する値を返す。
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="length"></param>
        /// <param name="fps"></param>
        /// <returns>0から1までのdouble</returns>
        public override double GetValue(long frame, long length, int fps)
        {
            double start = Start.GetValue(frame, length, fps);
            double time = frame / (double)fps - start;

            if (time < 0)
                return 1.0;

            double interval = Interval.GetValue(frame, length, fps);
            double transition = Transition.GetValue(frame, length, fps);

            double time2 = time % (transition + interval);

            if(time2 < transition)
                return 1.0 - time2 / transition;

            return 1.0;
        }

        public PeriodicParameter()
        {
        }

        public PeriodicParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyFrom(LayerValueExtraBase? origin)
        {
            var origin2 = origin as PeriodicParameter;
            if (origin2 == null) return;
            Start.CopyFrom(origin2.Start);
            Interval.CopyFrom(origin2.Interval);
            Transition.CopyFrom(origin2.Transition);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Start, Interval, Transition];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new StartSharedData(this));
            store.Save(new IntervalSharedData(this));
            store.Save(new TransitionSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<StartSharedData>() is StartSharedData startParameter)
                startParameter.CopyTo(this);
            if (store.Load<IntervalSharedData>() is IntervalSharedData intervalParameter)
                intervalParameter.CopyTo(this);
            if (store.Load<TransitionSharedData>() is TransitionSharedData transitionParameter)
                transitionParameter.CopyTo(this);
        }
    }
}
