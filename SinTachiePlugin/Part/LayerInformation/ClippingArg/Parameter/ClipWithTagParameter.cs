using SinTachiePlugin.Part.LayerInformation.ClippingArg.Argment.PartToClipTo;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.ClippingArg.Parameter
{
    internal class ClipWithTagParameter : ClippingArgBase, IPartToClipToParameter
    {
        [Display(Name = nameof(Texts.PartParam_LayerInfo_SourseSelectArg_PartToClipTo), ResourceType = typeof(Texts))]
        [TextEditor]
        public string PartToClipTo { get => partToClipTo; set => Set(ref partToClipTo, value); }
        private string partToClipTo = string.Empty;

        public ClipWithTagParameter()
        {
        }
        public ClipWithTagParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyFrom(ClippingArgBase? origin)
        {
            if (origin is IPartToClipToParameter partToClipToParameter)
                PartToClipTo = partToClipToParameter.PartToClipTo;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new PartToClipToSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<PartToClipToSharedData>() is PartToClipToSharedData partToClipToSharedData)
                partToClipToSharedData.CopyTo(this);
        }
    }
}
