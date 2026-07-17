namespace SinTachiePlugin.Draw.InverseKinematics
{
    internal record IKPath(PartNode Root, PartNode[] R2J, PartNode Joint, PartNode[] J2P, PartNode PreEndEffect, PartNode EndEffect)
    {
        public PartNode[] GetUnsettleds()
        {
            return [Root, .. R2J, Joint, .. J2P, PreEndEffect];
        }
    }
}
