using System.ComponentModel.DataAnnotations;
using SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment.Argument.TargetAnimationTag;
using SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment.Argument.TargetPartTag;
using SinTachiePlugin.Properties;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment.Parameter
{
    internal class DoLinkParameter : PartAnimationLinkArgBase, ITargetPartTagParameter, ITargetAnimationTagParameter
    {
        [Display(Name = nameof(TextResource.PartAnimationLink_TargetPartTag), ResourceType = typeof(TextResource))]
        [TextEditor]
        public string TargetPartTag { get => targetPartTag; set => Set(ref targetPartTag, value); }
        private string targetPartTag = string.Empty;

        [Display(Name = nameof(TextResource.PartAnimationLink_TargetAnimationTag), ResourceType = typeof(TextResource))]
        [TextEditor]
        public string TargetAnimationTag { get => targetAnimationTag; set => Set(ref targetAnimationTag, value); }
        private string targetAnimationTag = string.Empty;

        public DoLinkParameter()
        {
        }

        public DoLinkParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyFrom(PartAnimationLinkArgBase? origin)
        {
            if (origin is ITargetPartTagParameter targetPartTagParameter)
                targetPartTagParameter.TargetPartTag = TargetPartTag;
            if (origin is ITargetAnimationTagParameter targetAnimationTagParameter)
                targetAnimationTagParameter.TargetAnimationTag = TargetAnimationTag;
        }

        public override PartAnimationLinkArgBase GetClone()
        {
            DoLinkParameter clone = new();
            clone.CopyFrom(this);
            return clone;
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
