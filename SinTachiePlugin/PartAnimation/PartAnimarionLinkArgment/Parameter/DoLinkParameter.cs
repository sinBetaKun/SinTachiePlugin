using System.ComponentModel.DataAnnotations;
using SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment.Argment.TargetAnimationTag;
using SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment.Argment.TargetPartTag;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.Abrir;
using SinTachiePlugin.Properties;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment.Parameter
{
    internal class DoLinkParameter : PartAnimationLinkArgBase, ITargetPartTagParameter, ITargetAnimationTagParameter
    {
        [Display(Name = nameof(Texts.PartAnimationLink_TargetPartTag), ResourceType = typeof(Texts))]
        [TextEditor]
        public string TargetPartTag { get => targetPartTag; set => Set(ref targetPartTag, value); }
        private string targetPartTag = string.Empty;

        [Display(Name = nameof(Texts.PartAnimationLink_TargetAnimationTag), ResourceType = typeof(Texts))]
        [TextEditor]
        public string TargetAnimationTag { get => targetAnimationTag; set => Set(ref targetAnimationTag, value); }
        private string targetAnimationTag = string.Empty;

        public DoLinkParameter()
        {
        }

        public DoLinkParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyTo(PartAnimationLinkArgBase? origin)
        {
            if (origin is ITargetPartTagParameter targetPartTagParameter)
                targetPartTagParameter.TargetPartTag = TargetPartTag;
            if (origin is ITargetAnimationTagParameter targetAnimationTagParameter)
                targetAnimationTagParameter.TargetAnimationTag = TargetAnimationTag;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];


        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new TargetPartTagSharedData(this));
            store.Save(new TargetAnimationTagSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<TargetPartTagSharedData>() is TargetPartTagSharedData targetPartTagSharedData)
                targetPartTagSharedData.CopyTo(this);
            if (store.Load<TargetAnimationTagSharedData>() is TargetAnimationTagSharedData targetAnimationTagSharedData)
                targetAnimationTagSharedData.CopyTo(this);
        }
    }
}
