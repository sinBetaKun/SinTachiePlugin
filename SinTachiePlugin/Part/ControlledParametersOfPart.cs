using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.CenterPoint.CenterPointArg;
using SinTachiePlugin.Part.CenterPoint.CenterPointArg.Parameter;
using SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgument.Parameter;
using SinTachiePlugin.Part.CustomPoint.CustomPointArg;
using SinTachiePlugin.Part.CustomPoint.CustomPointArg.Parameter;
using SinTachiePlugin.Part.Drawing.DrawingArg;
using SinTachiePlugin.Part.Drawing.DrawingArg.Parameter;
using SinTachiePlugin.Part.Drawing.DrawingArg.SubArgument.Parameter;
using SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg;
using SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.Parameter;
using SinTachiePlugin.Part.LayerInformation.ClippingArg.Parameter;
using SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg.Parameter;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter;
using SinTachiePlugin.Part.Origin.OriginArg;
using SinTachiePlugin.Part.Origin.OriginArg.Parameter;
using SinTachiePlugin.Part.PartEffect.PartEffectArg;
using SinTachiePlugin.Part.PartEffect.PartEffectArg.Parameter;
using SinTachiePlugin.Part.ValueDependent.ValueDependentArg;
using SinTachiePlugin.Part.ValueDependent.ValueDependentArg.Parameter;
using SinTachiePlugin.PartAnimation;
using SinTachiePlugin.Parts;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part
{
    public class ControlledParametersOfPart : Animatable
    {
        [Display(GroupName = nameof(TextResource.GroupName_LayerInfo), Name = nameof(TextResource.PartParam_LayerInfo_Hide), ResourceType = typeof(TextResource))]
        [ToggleSlider]
        public bool Hide { get => _hide; set => Set(ref _hide, value); }
        private bool _hide = false;

        [Display(GroupName = nameof(TextResource.GroupName_LayerInfo), Name = nameof(TextResource.PartParam_LayerInfo_LayerType), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public LayerType ThisLayerType { get => _layerType; set => Set(ref _layerType, value); }
        private LayerType _layerType = LayerType.Image;

        [Display(GroupName = nameof(TextResource.GroupName_LayerInfo), Name = nameof(TextResource.PartParam_LayerInfo_Tag), ResourceType = typeof(TextResource))]
        [TextEditor]
        public string Tag { get => _tag; set => Set(ref _tag, value); }
        private string _tag = string.Empty;

        [Display(GroupName = nameof(TextResource.GroupName_LayerInfo), AutoGenerateField = true, ResourceType = typeof(TextResource))]
        public SourceSelectArgBase SourceSelectArg { get => _sourceSelectArg; set => Set(ref _sourceSelectArg, value); }
        private SourceSelectArgBase _sourceSelectArg = new ImageFileParameter();

        [Display(GroupName = nameof(TextResource.GroupName_LayerInfo), Name = nameof(TextResource.PartParam_LayerInfo_Comment), ResourceType = typeof(TextResource))]
        [TextEditor]
        public string Comment { get => _comment; set => Set(ref _comment, value); }
        private string _comment = string.Empty;

        [Display(GroupName = nameof(TextResource.GroupName_IK), AutoGenerateField = true, ResourceType = typeof(TextResource))]
        public InverseKinematicsArgBase InverseKinematicsArg { get => _inverseKinematicsArg; set => Set(ref _inverseKinematicsArg, value); }
        private InverseKinematicsArgBase _inverseKinematicsArg = new HavingSourceInverseKinematicsParameter();

        [Display(GroupName = nameof(TextResource.GroupName_Origin), AutoGenerateField = true, ResourceType = typeof(TextResource))]
        public OriginArgBase OriginArg { get => _originArg; set => Set(ref _originArg, value); }
        private OriginArgBase _originArg = new HavingSourceOriginParameter();

        [Display(GroupName = nameof(TextResource.GroupName_Drawing), AutoGenerateField = true, ResourceType = typeof(TextResource))]
        public DrawingArgBase DrawingArg { get => _drawingArg; set => Set(ref _drawingArg, value); }
        private DrawingArgBase _drawingArg = new HavingSourceDrawingParameter();

        [Display(GroupName = nameof(TextResource.GroupName_CenterPoint), AutoGenerateField = true, ResourceType = typeof(TextResource))]
        public CenterPointArgBase CenterPointArg { get => _centerPointArg; set => Set(ref _centerPointArg, value); }
        private CenterPointArgBase _centerPointArg = new HavingSourceCenterPointParameter();

        [Display(GroupName = nameof(TextResource.GroupName_CustomPoint), AutoGenerateField = true, ResourceType = typeof(TextResource))]
        public CustomPointArgBase CustomPointArg { get => _customPointArg; set => Set(ref _customPointArg, value); }
        private CustomPointArgBase _customPointArg = new HavingSourceCustomPointParameter();

        [Display(GroupName = nameof(TextResource.GroupName_ValueDependent), AutoGenerateField = true, ResourceType = typeof(TextResource))]
        public ValueDependentArgBase ValueDependentArg { get => _valueDependentArg; set => Set(ref _valueDependentArg, value); }
        private ValueDependentArgBase _valueDependentArg = new HavingSourceValueDependentParameter();

        [Display(GroupName = nameof(TextResource.GroupName_PartEffect), AutoGenerateField = true, ResourceType = typeof(TextResource))]
        public PartEffectArgBase PartEffectArg { get => _partEffectArg; set => Set(ref _partEffectArg, value); }
        private PartEffectArgBase _partEffectArg = new HavingSourcePartEffectParameter();

        public event EventHandler? LayerTypeChanged;

        public ControlledParametersOfPart()
        {
        }

        public ControlledParametersOfPart(ControlledParametersOfPart origin)
        {
            Hide = origin.Hide;
            ThisLayerType = origin.ThisLayerType;
            Tag = origin.Tag;
            Comment = origin.Comment;

            SourceSelectArg = origin.SourceSelectArg.GetClone();
            InverseKinematicsArg = origin.InverseKinematicsArg.GetClone();
            OriginArg = origin.OriginArg.GetClone();
            DrawingArg = origin.DrawingArg.GetClone();
            CenterPointArg = origin.CenterPointArg.GetClone();
            CustomPointArg = origin.CustomPointArg.GetClone();
            ValueDependentArg = origin.ValueDependentArg.GetClone();
            PartEffectArg = origin.PartEffectArg.GetClone();
        }

        [Obsolete]
        public ControlledParametersOfPart(PartBlock block)
        {
            #region LayerInfo
            Hide = !block.Appear;
            ThisLayerType = LayerType.Image;
            Tag = block.TagName;
            ImageFileParameter ip = new ImageFileParameter()
            {
                Parent = block.Parent,
                ImageFilePath = block.ImagePath,
                ClippingMode = ClippingMode.DontClip,
                ClippingArg = new DontClipParameter()
            };

            if (block.LayerValues.Count > 1)
            {
                MultiValuesParameter multiValuesParameter = new()
                {
                    PartAnimationValues = [.. block.LayerValues.Select(v => new PartAnimationValueExtra(v))]
                };

                for (int i = 0; i < multiValuesParameter.PartAnimationValues.Count; i++)
                {
                    multiValuesParameter.PartAnimationValues[i].Index = i + 1;
                }

                ip.PartAnimationValueMode = PartAnimationValueMode.Multi;
                ip.PartAnimationValueArg = multiValuesParameter;
                
            }
            else if (block.LayerValues.Count > 0)
            {
                SingleValueParameter singleValueParameter = new()
                {
                    PartAnimationValue = new(block.LayerValues.First())
                };
                ip.PartAnimationValueMode = PartAnimationValueMode.Single;
                ip.PartAnimationValueArg = singleValueParameter;
            }

            SourceSelectArg = ip;

            Comment = block.Comment;
            #endregion

            #region Origin
            HavingSourceOriginParameter hsop = new()
            {
                OriginMode = OriginDefineMode.CenterOfParent,
            };
            OriginArg = hsop;
            #endregion

            #region Drawing
            MasterModeParameter mmp = new();
            mmp.X.CopyFrom(block.X);
            mmp.Y.CopyFrom(block.Y);
            mmp.Z.CopyFrom(block.Z);
            mmp.Opacity.CopyFrom(block.Opacity);
            mmp.Zoom.CopyFrom(block.Scale);
            mmp.Rotation.CopyFrom(block.Rotate);
            mmp.Invert.CopyFrom(block.Mirror);
            mmp.Blend = block.BlendMode switch
            {
                BlendSTP.Dissolve => Blend.Dissolve,
                BlendSTP.Darken => Blend.Darker,
                BlendSTP.Multiply => Blend.Multiply,
                BlendSTP.ColorBurn => Blend.ColorBurn,
                BlendSTP.LinearBurn => Blend.LinearBurn,
                BlendSTP.Lighten => Blend.Lighter,
                BlendSTP.Screen => Blend.Screen,
                BlendSTP.ColorDodge => Blend.ColorDodge,
                BlendSTP.LinearDodge => Blend.LinearDodge,
                BlendSTP.Plus => Blend.Add,
                BlendSTP.Overlay => Blend.Overlay,
                BlendSTP.SoftLight => Blend.SoftLight,
                BlendSTP.HardLight => Blend.HardLight,
                BlendSTP.VividLight => Blend.VividLight,
                BlendSTP.LinearLight => Blend.LinearLight,
                BlendSTP.PinLight => Blend.PinLight,
                BlendSTP.HardMix => Blend.HardMix,
                BlendSTP.Difference => Blend.Difference,
                BlendSTP.Exclusion => Blend.Exclusion,
                BlendSTP.Subtract => Blend.Subtract,
                BlendSTP.Division => Blend.Division,
                BlendSTP.Hue => Blend.Hue,
                BlendSTP.Saturation => Blend.Saturation,
                BlendSTP.Color => Blend.Color,
                BlendSTP.Luminosity => Blend.Luminosity,
                BlendSTP.LighterColor => Blend.LighterColor,
                BlendSTP.DestinationOver => Blend.DestinationOver,
                BlendSTP.DarkerColor => Blend.DarkerColor,
                BlendSTP.DestinationOut => Blend.DestinationOut,
                BlendSTP.SourceAtop => Blend.SourceAtop,
                BlendSTP.XOR => Blend.XOR,
                BlendSTP.MaskInverseErt => Blend.MaskInvert,
                _ => Blend.Normal
            };
            mmp.ZSort = block.ZSortMode switch
            {
                ZSortMode.Ignore => ZSortMode2.IgnoreZ,
                ZSortMode.GlobalSpace => ZSortMode2.IgnorePriority,
                _ => ZSortMode2.BasedOnPriority
            };
            mmp.Zoom_X.CopyFrom(block.Exp_X);
            mmp.Zoom_Y.CopyFrom(block.Exp_Y);
            HavingSourceDrawingParameter dp = new()
            {
                ModeMaster = DrawingValueModeMaster.Override,
                SubArg = mmp
            };
            DrawingArg = dp;
            #endregion

            #region CenterPoint
            OnlyCoordinateParameter ocp = new();
            ocp.X.CopyFrom(block.Cnt_X);
            ocp.Y.CopyFrom(block.Cnt_Y);
            ocp.KeepPlace = block.KeepPlace;
            CenterPointArg = new HavingSourceCenterPointParameter()
            {
                CenterMode = CenterPointMode.DontSet,
                SubArg = ocp
            };
            #endregion

            #region ValueDependent
            HavingSourceValueDependentParameter vdp = new();

            if (block.XYZDependent && block.OpacityDependent && block.ScaleDependent && block.RotateDependent && block.MirrorDependent && block.CameraDependent && block.UnlazyEffectDependent)
            {
                vdp.ModeMaster = ValueDependentModeMaster.On;
            }
            else if (!block.XYZDependent && !block.OpacityDependent && !block.ScaleDependent && !block.RotateDependent && !block.MirrorDependent && !block.CameraDependent && !block.UnlazyEffectDependent)
            {
                vdp.ModeMaster = ValueDependentModeMaster.Off;
            }
            else
            {
                vdp.ModeMaster = ValueDependentModeMaster.Custom;
                ValueDependent.ValueDependentArg.SubArgument.Parameter.CustomModeParameter cmp = new()
                {
                    XYZ = block.XYZDependent ? ValueDependentMode.On : ValueDependentMode.Off,
                    Opacity = block.OpacityDependent ? ValueDependentMode.On : ValueDependentMode.Off,
                    Zoom = block.ScaleDependent ? ValueDependentMode.On : ValueDependentMode.Off,
                    Rotation = block.RotateDependent ? ValueDependentMode.On : ValueDependentMode.Off,
                    Invert = block.MirrorDependent ? ValueDependentMode.On : ValueDependentMode.Off,
                    Camera = block.CameraDependent ? ValueDependentMode.On : ValueDependentMode.Off,
                    UnlazyEffect = block.UnlazyEffectDependent ? ValueDependentMode.On : ValueDependentMode.Off
                };
                vdp.SubArg = cmp;
            }
            
            ValueDependentArg = vdp;
            #endregion

            #region PartEffect
            HavingSourcePartEffectParameter pep = new()
            {
                Effects = YukkuriMovieMaker.Json.Json.GetClone(block.Effects)!
            };
            PartEffectArg = pep;
            #endregion
        }

        private LayerType oldType = LayerType.Image;

        public override ValueTask EndEditAsync()
        {
            SourceSelectArg = ThisLayerType.Convert(SourceSelectArg);
            InverseKinematicsArg = ThisLayerType.Convert(InverseKinematicsArg);
            OriginArg = ThisLayerType.Convert(OriginArg);
            DrawingArg = ThisLayerType.Convert(DrawingArg);
            CenterPointArg = ThisLayerType.Convert(CenterPointArg);
            CustomPointArg = ThisLayerType.Convert(CustomPointArg);
            ValueDependentArg = ThisLayerType.Convert(ValueDependentArg);
            PartEffectArg = ThisLayerType.Convert(PartEffectArg);

            if (oldType != ThisLayerType)
            {
                LayerTypeChanged?.Invoke(this, EventArgs.Empty);
                oldType = ThisLayerType;
            }

            return base.EndEditAsync();
        }

        public void SwitchType(LayerType type)
        {
            ThisLayerType = type;
            SourceSelectArg = ThisLayerType.Convert(SourceSelectArg);
            InverseKinematicsArg = ThisLayerType.Convert(InverseKinematicsArg);
            OriginArg = ThisLayerType.Convert(OriginArg);
            DrawingArg = ThisLayerType.Convert(DrawingArg);
            CenterPointArg = ThisLayerType.Convert(CenterPointArg);
            CustomPointArg = ThisLayerType.Convert(CustomPointArg);
            ValueDependentArg = ThisLayerType.Convert(ValueDependentArg);
            PartEffectArg = ThisLayerType.Convert(PartEffectArg);
            oldType = ThisLayerType;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => 
            [SourceSelectArg, InverseKinematicsArg, OriginArg, DrawingArg, CenterPointArg, CustomPointArg, ValueDependentArg, PartEffectArg];
    }
}
