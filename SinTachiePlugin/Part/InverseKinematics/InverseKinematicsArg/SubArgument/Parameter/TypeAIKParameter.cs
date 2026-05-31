using SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Argument.EndEffectorPartTag;
using SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Argument.EndEffectorPointName;
using SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Argument.EndEffectorPointXY;
using SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Argument.JointPartTag;
using SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Argument.PreEndEffectorPointName;
using SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Argument.PreEndEffectorPointXY;
using SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Argument.RootPartTag;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Parameter
{
    internal class TypeAIKParameter : InverseKinematicsSubArgBase, IRootPartTagParameter, IJointPartTagParameter, IEndEffectorPartTagParameter, IEndEffectorPointNameParameter, IEndEffectorPointXYParameter, IPreEndEffectorPointNameParameter, IPreEndEffectorPointXYParameter
    {
        [Display(Name = nameof(TextResource.InverseKinematics_RootPartTag), ResourceType = typeof(TextResource))]
        [TextEditor]
        public string RootPartTag { get => _rootPartTag; set => Set(ref _rootPartTag, value); }
        private string _rootPartTag = string.Empty;

        [Display(Name = nameof(TextResource.InverseKinematics_JointPartTag), ResourceType = typeof(TextResource))]
        [TextEditor]
        public string  JointPartTag { get => _jointPartTag; set => Set(ref _jointPartTag, value); }
        private string _jointPartTag = string.Empty;

        [Display(Name = nameof(TextResource.InverseKinematics_EndEffectorPartTag), ResourceType = typeof(TextResource))]
        [TextEditor]
        public string EndEffectorPartTag { get => _endEffectorPartTag; set => Set(ref _endEffectorPartTag, value); }
        private string _endEffectorPartTag = string.Empty;

        [Display(Name = nameof(TextResource.InverseKinematics_EndEffectorPointName), ResourceType = typeof(TextResource))]
        [TextEditor]
        public string EndEffectorPointName { get => _endEffectorPointName; set => Set(ref _endEffectorPointName, value); }
        private string _endEffectorPointName = string.Empty;

        [Display(Name = nameof(TextResource.InverseKinematics_EndEffectorPointX), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation EndEffectorPointX { get; } = new(0, -10000, 10000);

        [Display(Name = nameof(TextResource.InverseKinematics_EndEffectorPointY), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation EndEffectorPointY { get; } = new(0, -10000, 10000);

        [Display(Name = nameof(TextResource.InverseKinematics_PreEndEffectorPointName), ResourceType = typeof(TextResource))]
        [TextEditor]
        public string PreEndEffectorPointName { get => _preEndEffectorPointName; set => Set(ref _preEndEffectorPointName, value); }
        private string _preEndEffectorPointName = string.Empty;

        [Display(Name = nameof(TextResource.InverseKinematics_PreEndEffectorPointX), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation PreEndEffectorPointX { get; } = new(0, -10000, 10000);

        [Display(Name = nameof(TextResource.InverseKinematics_PreEndEffectorPointY), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation PreEndEffectorPointY { get; } = new(0, -10000, 10000);

        public TypeAIKParameter()
        {
        }

        public TypeAIKParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(InverseKinematicsSubArgBase? origin)
        {
            if (origin is IRootPartTagParameter rootPartTagParameter)
                RootPartTag = rootPartTagParameter.RootPartTag;
            if (origin is IJointPartTagParameter jointPartTagParameter)
                JointPartTag = jointPartTagParameter.JointPartTag;
            if (origin is IEndEffectorPartTagParameter endEffectorPartTagParameter)
                EndEffectorPartTag = endEffectorPartTagParameter.EndEffectorPartTag;
            if (origin is IEndEffectorPointNameParameter endEffectorPointNameParameter)
                EndEffectorPointName = endEffectorPointNameParameter.EndEffectorPointName;
            if (origin is IEndEffectorPointXYParameter endEffectorPointXYParameter)
            {
                EndEffectorPointX.CopyFrom(endEffectorPointXYParameter.EndEffectorPointX);
                EndEffectorPointY.CopyFrom(endEffectorPointXYParameter.EndEffectorPointY);
            }
            if (origin is IPreEndEffectorPointNameParameter preEndEffectorPointNameParameter)
                PreEndEffectorPointName = preEndEffectorPointNameParameter.PreEndEffectorPointName;
            if (origin is IPreEndEffectorPointXYParameter preEndEffectorPointXYParameter)
            {
                PreEndEffectorPointX.CopyFrom(preEndEffectorPointXYParameter.PreEndEffectorPointX);
                PreEndEffectorPointY.CopyFrom(preEndEffectorPointXYParameter.PreEndEffectorPointY);
            }
        }

        public override InverseKinematicsSubArgBase GetClone()
        {
            TypeAIKParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [EndEffectorPointX, EndEffectorPointY, PreEndEffectorPointX, PreEndEffectorPointY];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new RootPartTagSharedData(this));
            store.Save(new JointPartTagSharedData(this));
            store.Save(new EndEffectorPartTagSharedData(this));
            store.Save(new EndEffectorPointNameSharedData(this));
            store.Save(new EndEffectorPointXYSharedData(this));
            store.Save(new PreEndEffectorPointNameSharedData(this));
            store.Save(new PreEndEffectorPointXYSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<RootPartTagSharedData>() is RootPartTagSharedData rootPartTagSharedData)
                rootPartTagSharedData.CopyTo(this);
            if (store.Load<JointPartTagSharedData>() is JointPartTagSharedData jointPartTagSharedData)
                jointPartTagSharedData.CopyTo(this);
            if (store.Load<EndEffectorPartTagSharedData>() is EndEffectorPartTagSharedData endEffectorPartTagSharedData)
                endEffectorPartTagSharedData.CopyTo(this);
            if (store.Load<EndEffectorPointNameSharedData>() is EndEffectorPointNameSharedData endEffectorPointNameSharedData)
                endEffectorPointNameSharedData.CopyTo(this);
            if (store.Load<EndEffectorPointXYSharedData>() is EndEffectorPointXYSharedData endEffectorPointXYSharedData)
                endEffectorPointXYSharedData.CopyTo(this);
            if (store.Load<PreEndEffectorPointNameSharedData>() is PreEndEffectorPointNameSharedData preEndEffectorPointNameSharedData)
                preEndEffectorPointNameSharedData.CopyTo(this);
            if (store.Load<PreEndEffectorPointXYSharedData>() is PreEndEffectorPointXYSharedData preEndEffectorPointXYSharedData)
                preEndEffectorPointXYSharedData.CopyTo(this);
        }
    }
}
