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

        private bool _prevHide = false;
        private readonly PGoPN_Source _prevSource = new();
        private readonly PGoPN_PartAnimationValue _prevPAV = new();
        private readonly PGoPN_LayerInformation _prevLI = new();
        private readonly PGoPN_InverseKinematics _prevIK = new();
        private readonly PGoPN_Origin _prevOrigin = new();
        private readonly PGoPN_Drawing _prevDrawing = new();
        private readonly PGoPN_CenterPoint _prevCenterPoint = new();
        private readonly PGoPN_CustomPoint _prevCustomPoint = new();
        private readonly PGoPN_ValueDependent _prevVD = new();
        private readonly PGoPN_PartEffect _prevPE = new();

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

        public void SyncPrev()
        {
            _prevHide = Hide;
            SourceGroup.CopyTo(_prevSource);
            PartAnimationValueGroup.CopyTo(_prevPAV);
            LayerInformationGroup.CopyTo(_prevLI);
            InverseKinematicsGroup.CopyTo(_prevIK);
            OriginGroup.CopyTo(_prevOrigin);
            DrawingGroup.CopyTo(_prevDrawing);
            CenterPointGroup.CopyTo(_prevCenterPoint);
            CustomPointGroup.CopyTo(_prevCustomPoint);
            ValueDependentGroup.CopyTo(_prevVD);
            PartEffectGroup.CopyTo(_prevPE);
        }

        public PartNodeComparateResult ComparateWithPrev()
        {
            return
                new PartNodeComparateResult(CommandList: Hide != _prevHide) |
                SourceGroup.Comparate(_prevSource) |
                PartAnimationValueGroup.Comparate(_prevPAV) |
                LayerInformationGroup.Comparate(_prevLI) |
                InverseKinematicsGroup.Comparate(_prevIK) |
                OriginGroup.Comparate(_prevOrigin) |
                DrawingGroup.Comparate(_prevDrawing) |
                CenterPointGroup.Comparate(_prevCenterPoint) |
                CustomPointGroup.Comparate(_prevCustomPoint) |
                ValueDependentGroup.Comparate(_prevVD) |
                PartEffectGroup.Comparate(_prevPE);
        }
    }
}
