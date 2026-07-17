using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.LayerInformation.ClippingArg;
using SinTachiePlugin.Part.LayerInformation.ClippingArg.Parameter;
using SinTachiePlugin.Part.LayerInformation.InsertArg;
using SinTachiePlugin.Part.LayerInformation.InsertArg.Parameter;
using SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg;
using SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg.Parameter;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.Clip;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.Insert;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.Parent;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.PartAnimationValue;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.PsdFileInfo;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Plugin.Tachie.Psd;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter
{
    internal class PsdFileParameter : SourceSelectArgBase, IParentParameter, IPsdFileInfoParameter, IClipParameter, IInsertParameter, IPartAnimationValueParameter
    {
        [Display(Name = nameof(TextResource.PartParam_LayerInfo_SourceSelectArg_Parent), ResourceType = typeof(TextResource))]
        [TextEditor]
        public string Parent { get => _parent; set => Set(ref _parent, value); }
        private string _parent = string.Empty;

        [Display(AutoGenerateField = true)]
        public PsdShapeParameter PsdFileInfo { get => _psdFileInfo; set => Set(ref _psdFileInfo, value); }
        private PsdShapeParameter _psdFileInfo = new();

        [Display(Name = nameof(TextResource.PartParam_LayerInfo_SourceSelectArg_ClippingMode), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public ClippingMode ClippingMode { get => _clippingMode; set => Set(ref _clippingMode, value); }
        private ClippingMode _clippingMode = ClippingMode.DontClip;

        [Display(AutoGenerateField = true)]
        public ClippingArgBase ClippingArg { get => _clippingArg; set => Set(ref _clippingArg, value); }
        private ClippingArgBase _clippingArg = new DontClipParameter();

        [Display(Name = nameof(TextResource.PartParam_LayerInfo_SourceSelectArg_InsertMode), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public PartInsertDefineMode InsertDefineMode { get => _insertDefineMode; set => Set(ref _insertDefineMode, value); }
        private PartInsertDefineMode _insertDefineMode = PartInsertDefineMode.DontInsert;

        [Display(AutoGenerateField = true)]
        public InsertArgBase InsertArg { get => _insertArg; set => Set(ref _insertArg, value); }
        private InsertArgBase _insertArg = new DontInsertParameter();

        [Display(Name = nameof(TextResource.PartParam_LayerInfo_SourceSelectArg_PartAnimationValueMode), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public PartAnimationValueMode PartAnimationValueMode { get => _partAnimationValueMode; set => Set(ref _partAnimationValueMode, value); }
        private PartAnimationValueMode _partAnimationValueMode = PartAnimationValueMode.None;

        [Display(AutoGenerateField = true)]
        public PartAnimationValueArgBase PartAnimationValueArg { get => _partAnimationValueArg; set => Set(ref _partAnimationValueArg, value); }
        private PartAnimationValueArgBase _partAnimationValueArg = new NoneValueParameter();

        public PsdFileParameter()
        {
        }

        public PsdFileParameter(SharedDataStore? store) : base(store)
        {
        }

        public override ValueTask EndEditAsync()
        {
            ClippingArg = ClippingMode.Convert(ClippingArg);
            InsertArg = InsertDefineMode.Convert(InsertArg);
            PartAnimationValueArg = PartAnimationValueMode.Convert(PartAnimationValueArg);
            return base.EndEditAsync();
        }

        public override void CopyFrom(SourceSelectArgBase? origin)
        {
            if (origin is IParentParameter parentParameter)
                Parent = parentParameter.Parent;
            if (origin is IPsdFileInfoParameter psdFileInfoParameter)
            {
                PsdFileInfo.FilePath = psdFileInfoParameter.PsdFileInfo.FilePath;
                PsdFileInfo.EnableLayersFilePath = psdFileInfoParameter.PsdFileInfo.EnableLayersFilePath;
                PsdFileInfo.EnableLayers = [.. psdFileInfoParameter.PsdFileInfo.EnableLayers];
            }
            if (origin is IClipParameter clippingParameter)
            {
                ClippingMode = clippingParameter.ClippingMode;
                ClippingArg = clippingParameter.ClippingArg.GetClone();
            }
            if (origin is IInsertParameter insertParameter)
            {
                InsertDefineMode = insertParameter.InsertDefineMode;
                InsertArg = insertParameter.InsertArg.GetClone();
            }
            if (origin is IPartAnimationValueParameter partAnimationValueParameter)
            {
                PartAnimationValueMode = partAnimationValueParameter.PartAnimationValueMode;
                PartAnimationValueArg = partAnimationValueParameter.PartAnimationValueArg.GetClone();
            }
        }

        public override SourceSelectArgBase GetClone()
        {
            PsdFileParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [ClippingArg, InsertArg, PartAnimationValueArg];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new ParentSharedData(this));
            store.Save(new PsdFileInfoSharedData(this));
            store.Save(new ClipSharedData(this));
            store.Save(new InsertSharedData(this));
            store.Save(new PartAnimationValueSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<ParentSharedData>() is ParentSharedData parentSharedData)
                parentSharedData.CopyTo(this);
            if (store.Load<PsdFileInfoSharedData>() is PsdFileInfoSharedData psdFileInfoSharedData)
                psdFileInfoSharedData.CopyTo(this);
            if (store.Load<ClipSharedData>() is ClipSharedData clipSharedData)
                clipSharedData.CopyTo(this);
            if (store.Load<InsertSharedData>() is InsertSharedData insertSharedData)
                insertSharedData.CopyTo(this);
            if (store.Load<PartAnimationValueSharedData>() is PartAnimationValueSharedData partAnimationValueSharedData)
                partAnimationValueSharedData.CopyTo(this);
        }
    }
}
