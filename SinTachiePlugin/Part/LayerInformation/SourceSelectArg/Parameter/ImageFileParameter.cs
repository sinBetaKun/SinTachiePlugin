using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.LayerInformation.ClippingArg;
using SinTachiePlugin.Part.LayerInformation.ClippingArg.Parameter;
using SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg;
using SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg.Parameter;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.Clip;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.ImageFilePath;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.Parent;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.PartAnimationValue;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter
{
    internal class ImageFileParameter : SourceSelectArgBase, IParentParameter, IImageFilePathParameter, IClipParameter, IPartAnimationValueParameter
    {
        [Display(Name = nameof(TextResource.PartParam_LayerInfo_SourceSelectArg_Parent), ResourceType = typeof(TextResource))]
        [TextEditor]
        public string Parent { get => _parent; set => Set(ref _parent, value); }
        private string _parent = string.Empty;

        [Display(Name = nameof(TextResource.PartParam_LayerInfo_SourceSelectArg_ImageFile), ResourceType = typeof(TextResource))]
        [FileSelector(YukkuriMovieMaker.Settings.FileGroupType.ImageItem)]
        public string ImageFilePath { get => _imageFilePath; set => Set(ref _imageFilePath, value); }
        private string _imageFilePath = string.Empty;

        [Display(Name = nameof(TextResource.PartParam_LayerInfo_SourceSelectArg_ClippingMode), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public ClippingMode ClippingMode { get => _clippingMode; set => Set(ref _clippingMode, value); }
        private ClippingMode _clippingMode = ClippingMode.DontClip;

        [Display(AutoGenerateField = true)]
        public ClippingArgBase ClippingArg { get => _clippingArg; set => Set(ref _clippingArg, value); }
        private ClippingArgBase _clippingArg = new DontClipParameter();

        [Display(Name = nameof(TextResource.PartParam_LayerInfo_SourceSelectArg_PartAnimationValueMode), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public PartAnimationValueMode PartAnimationValueMode { get => _partAnimationValueMode; set => Set(ref _partAnimationValueMode, value); }
        private PartAnimationValueMode _partAnimationValueMode = PartAnimationValueMode.None;

        [Display(AutoGenerateField = true)]
        public PartAnimationValueArgBase PartAnimationValueArg { get => _partAnimationValueArg; set => Set(ref _partAnimationValueArg, value); }
        private PartAnimationValueArgBase _partAnimationValueArg = new NoneValueParameter();

        public ImageFileParameter()
        {
        }

        public ImageFileParameter(SharedDataStore? store) : base(store)
        {
        }
        
        public override ValueTask EndEditAsync()
        {
            ClippingArg = ClippingMode.Convert(ClippingArg);
            PartAnimationValueArg = PartAnimationValueMode.Convert(PartAnimationValueArg);
            return base.EndEditAsync();
        }

        public override void CopyFrom(SourceSelectArgBase? origin)
        {
            if (origin is IParentParameter parentParameter)
                Parent = parentParameter.Parent;
            if (origin is IImageFilePathParameter imageFilePathParameter)
                ImageFilePath = imageFilePathParameter.ImageFilePath;
            if (origin is IClipParameter clippingParameter)
            {
                ClippingMode = clippingParameter.ClippingMode;
                ClippingArg = clippingParameter.ClippingArg.GetClone();
            }
            if (origin is IPartAnimationValueParameter partAnimationValueParameter)
            {
                PartAnimationValueMode = partAnimationValueParameter.PartAnimationValueMode;
                PartAnimationValueArg = partAnimationValueParameter.PartAnimationValueArg.GetClone();
            }
        }

        public override SourceSelectArgBase GetClone()
        {
            ImageFileParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [ClippingArg, PartAnimationValueArg];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new ParentSharedData(this));
            store.Save(new ImageFilePathSharedData(this));
            store.Save(new ClipSharedData(this));
            store.Save(new PartAnimationValueSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<ParentSharedData>() is ParentSharedData parentSharedData)
                parentSharedData.CopyTo(this);
            if (store.Load<ImageFilePathSharedData>() is ImageFilePathSharedData imageFilePathSharedData)
                imageFilePathSharedData.CopyTo(this);
            if (store.Load<ClipSharedData>() is ClipSharedData clipSharedData)
                clipSharedData.CopyTo(this);
            if (store.Load<PartAnimationValueSharedData>() is PartAnimationValueSharedData partAnimationValueSharedData)
                partAnimationValueSharedData.CopyTo(this);
        }
    }
}
