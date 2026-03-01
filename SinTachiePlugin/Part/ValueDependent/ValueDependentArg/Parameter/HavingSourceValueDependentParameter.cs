using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.ValueDependent.ValueDependentArg.Argment.DependentModeOfValues;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.ValueDependent.ValueDependentArg.Parameter
{
    internal class HavingSourceValueDependentParameter : ValueDependentArgBase, IDependentModeOfValueParameter
    {
        [Display(Name = nameof(TextResource.PartParam_ValueDependentMode_XYZ), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public ValueDependentMode XYZ { get => _xyz; set => Set(ref _xyz, value); }
        private ValueDependentMode _xyz = ValueDependentMode.On;

        [Display(Name = nameof(TextResource.PartParam_ValueDependentMode_Opacity), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public ValueDependentMode Opacity { get => _opacity; set => Set(ref _opacity, value); }
        private ValueDependentMode _opacity = ValueDependentMode.On;

        [Display(Name = nameof(TextResource.PartParam_ValueDependentMode_Zoom), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public ValueDependentMode Zoom { get => _zoom; set => Set(ref _zoom, value); }
        private ValueDependentMode _zoom = ValueDependentMode.On;

        [Display(Name = nameof(TextResource.PartParam_ValueDependentMode_Rotation), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public ValueDependentMode Rotation { get => _rotation; set => Set(ref _rotation, value); }
        private ValueDependentMode _rotation = ValueDependentMode.On;

        [Display(Name = nameof(TextResource.PartParam_ValueDependentMode_Invert), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public ValueDependentMode Invert { get => _invert; set => Set(ref _invert, value); }
        private ValueDependentMode _invert = ValueDependentMode.On;

        [Display(Name = nameof(TextResource.PartParam_ValueDependentMode_Camera), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public ValueDependentMode Camera { get => _camera; set => Set(ref _camera, value); }
        private ValueDependentMode _camera = ValueDependentMode.On;

        [Display(Name = nameof(TextResource.PartParam_ValueDependentMode_UnlazyEffect), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public ValueDependentMode UnlazyEffect { get => _unlazyEffect; set => Set(ref _unlazyEffect, value); }
        private ValueDependentMode _unlazyEffect = ValueDependentMode.On;

        public HavingSourceValueDependentParameter()
        {
        }

        public HavingSourceValueDependentParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(ValueDependentArgBase? origin)
        {
            if (origin is IDependentModeOfValueParameter dependentModeOfValueParameter)
            {
                XYZ = dependentModeOfValueParameter.XYZ;
                Opacity = dependentModeOfValueParameter.Opacity;
                Zoom = dependentModeOfValueParameter.Zoom;
                Rotation = dependentModeOfValueParameter.Rotation;
                Invert = dependentModeOfValueParameter.Invert;
                Camera = dependentModeOfValueParameter.Camera;
                UnlazyEffect = dependentModeOfValueParameter.UnlazyEffect;
            }
        }

        public override ValueDependentArgBase GetClone()
        {
            HavingSourceValueDependentParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new DependentModeOfValuesSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<DependentModeOfValuesSharedData>() is DependentModeOfValuesSharedData dependentModeOfValuesSharedData)
            {
                XYZ = dependentModeOfValuesSharedData.XYZ;
                Opacity = dependentModeOfValuesSharedData.Opacity;
                Zoom = dependentModeOfValuesSharedData.Zoom;
                Rotation = dependentModeOfValuesSharedData.Rotation;
                Invert = dependentModeOfValuesSharedData.Invert;
                Camera = dependentModeOfValuesSharedData.Camera;
                UnlazyEffect = dependentModeOfValuesSharedData.UnlazyEffect;
            }
        }
    }
}
