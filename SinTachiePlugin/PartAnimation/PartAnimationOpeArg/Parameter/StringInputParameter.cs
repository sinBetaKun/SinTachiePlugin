using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.StringInput;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Parameter
{
    internal class StringInputParameter : PartAnimationOpeArgBase, IStringInputParameter
    {
        [Display(Name = nameof(TextResource.PartAnimationOpeArg_StringInput), ResourceType = typeof(TextResource))]
        [TextEditor]
        public string StringInput { get; set; } = string.Empty;

        public override PartAnimationResultA GetResult(TachieSourceDescription desc)
        {
            return PartAnimationResultA.FromText(StringInput);
        }

        public StringInputParameter()
        {
        }

        public StringInputParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyFrom(PartAnimationOpeArgBase? origin)
        {
            if (origin is IStringInputParameter stringInputParameter)
                StringInput = stringInputParameter.StringInput;
        }

        public override PartAnimationOpeArgBase GetClone()
        {
            StringInputParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new StringInputSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<StringInputSharedData>() is StringInputSharedData stringInputSharedData)
                StringInput = stringInputSharedData.StringInput;
        }
    }
}
