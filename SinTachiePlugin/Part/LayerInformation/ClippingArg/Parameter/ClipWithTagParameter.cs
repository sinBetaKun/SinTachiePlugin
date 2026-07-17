using SinTachiePlugin.Part.LayerInformation.ClippingArg.Argument.TagToClipTo;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.ClippingArg.Parameter
{
    internal class ClipWithTagParameter : ClippingArgBase, ITagToClipToParameter
    {
        [Display(Name = nameof(TextResource.PartParam_LayerInfo_ClippingArg_TagToClipTo), ResourceType = typeof(TextResource))]
        [TextEditor]
        public string TagToClipTo { get => partToClipTo; set => Set(ref partToClipTo, value); }
        private string partToClipTo = string.Empty;

        public ClipWithTagParameter()
        {
        }
        public ClipWithTagParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyFrom(ClippingArgBase? origin)
        {
            if (origin is ITagToClipToParameter partToClipToParameter)
                TagToClipTo = partToClipToParameter.TagToClipTo;
        }

        public override ClippingArgBase GetClone()
        {
            ClipWithTagParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new TagToClipToSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<TagToClipToSharedData>() is TagToClipToSharedData partToClipToSharedData)
                partToClipToSharedData.CopyTo(this);
        }
    }
}
