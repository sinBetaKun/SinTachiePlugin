using SinTachiePlugin.Draw.ParamGroupOfPartNode;
using SinTachiePlugin.Enums;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.Draw
{
    internal class ParamsOfPartNode
    {
        public bool Hide { get; private set; } = false;
        public PGoPN_Source SourceGroup { get; private set; } = new();
        public PGoPN_PartAnimationValue PartAnimationValueGroup { get; private set; } = new();
        public PGoPN_LayerInformation LayerInformationGroup { get; private set; } = new();
        public PGoPN_InverseKinematics InverseKinematicsGroup { get; private set; } = new();
        public PGoPN_Origin OriginGroup { get; private set; } = new();
        public PGoPN_Drawing DrawingGroup { get; private set; } = new();
        public PGoPN_CenterPoint CenterPointGroup { get; private set; } = new();
        public PGoPN_CustomPoint CustomPointGroup { get; private set; } = new();
        public PGoPN_ValueDependent ValueDependentGroup { get; private set; } = new();
        public PGoPN_PartEffect PartEffectGroup { get; private set; } = new();


        public void Update(List<PartValueAndFL> pvfls, TachieSourceDescription desc)
        {
            if (pvfls.Count == 0)
                return;

            Hide = pvfls.Last().PartValue.ControlledParameters.Hide;

            SourceGroup.Update(pvfls, desc);

            if (SourceGroup.ThisLayerType == LayerType.Image)
                PartAnimationValueGroup.Update(pvfls, desc);
            else
                PartAnimationValueGroup.Reset();

            if (SourceGroup.ThisLayerType == LayerType.Group)
            {
                LayerInformationGroup.Reset();
                InverseKinematicsGroup.Reset();
                OriginGroup.Reset();
                DrawingGroup.Reset();
                CenterPointGroup.Reset();
                CustomPointGroup.Reset();
                ValueDependentGroup.Reset();
                PartEffectGroup.Reset();
            }
            else
            {
                LayerInformationGroup.Update(pvfls);
                InverseKinematicsGroup.Update(pvfls);
                OriginGroup.Update(pvfls);
                DrawingGroup.Update(pvfls, desc);
                CenterPointGroup.Update(pvfls, desc);
                CustomPointGroup.Update(pvfls);
                ValueDependentGroup.Update(pvfls);
                PartEffectGroup.Update(pvfls, desc);
            }
        }
    }
}
