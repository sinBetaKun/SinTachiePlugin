using SinTachiePlugin.Draw.ParamGroupOfPartNode;
using SinTachiePlugin.Draw.PointParam;
using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgument;
using System.Collections.Immutable;
using System.Numerics;

namespace SinTachiePlugin.Draw.InverseKinematics
{
    internal static class IKPathManager
    {
        public static void OverwriteDrawDescriptionForIK(IEnumerable<PartNode> pns)
        {
            #region BuildIKPath
            PartNode[] overall = [.. pns];
            List<IKPath> ikp_l0 = [];
            string[] s_4tag_a0 = [.. overall.Select(x => x.Tag).Where(x => !string.IsNullOrEmpty(x))];

            foreach (PartNode pn_0 in overall)
            {
                if (pn_0.Params.SourceGroup.ThisLayerType == LayerType.Group)
                    continue;

                PGoPN_InverseKinematics ikg_0 = pn_0.Params.InverseKinematicsGroup;

                if (string.IsNullOrEmpty(ikg_0.RootPartTag)
                    || string.IsNullOrEmpty(ikg_0.JointPartTag))
                {
                    continue;
                }

                if (ikg_0.RootPartTag == pn_0.Tag
                    || ikg_0.JointPartTag == pn_0.Tag
                    || ikg_0.EndEffectorPartTag == pn_0.Tag)
                {
                    continue;
                }

                if (ikg_0.RootPartTag == ikg_0.JointPartTag
                    || ikg_0.RootPartTag == ikg_0.EndEffectorPartTag
                    || ikg_0.EndEffectorPartTag == ikg_0.JointPartTag)
                {
                    continue;
                }

                ImmutableList<PartNode> pn_l0 = pn_0.Ancestors;
                string[] s_4tag_a1 = [.. pn_l0.Select(x => x.Tag)];

                if (!s_4tag_a1.Contains(ikg_0.RootPartTag)
                    || !s_4tag_a1.Contains(ikg_0.JointPartTag)
                    || !(s_4tag_a0.Contains(ikg_0.EndEffectorPartTag) || string.IsNullOrEmpty(ikg_0.EndEffectorPartTag))
                    || s_4tag_a1.Contains(ikg_0.EndEffectorPartTag))
                {
                    continue;
                }

                int i_4idx_0 = s_4tag_a1.IndexOf(ikg_0.RootPartTag);
                int i_4idx_1 = s_4tag_a1.IndexOf(ikg_0.JointPartTag);

                if (i_4idx_0 < i_4idx_1)
                    continue;

                PartNode[] pn_a0 = [.. pn_l0.GetRange(i_4idx_1 + 1, i_4idx_0 - i_4idx_1 - 1).Reverse()];
                PartNode[] pn_a1 = [.. pn_l0.GetRange(0, i_4idx_1).Reverse()];
                PartNode? pn_4eepn_0 = null;

                if (!string.IsNullOrEmpty(ikg_0.EndEffectorPartTag)) pn_4eepn_0 = overall.First(x => x.Tag == ikg_0.EndEffectorPartTag);

                ikp_l0.Add(new(pn_l0[i_4idx_0], pn_a0, pn_l0[i_4idx_1], pn_a1, pn_0, pn_4eepn_0));
            }

            List<PartNode> pn_l1 = [.. ikp_l0.SelectMany(x => x.GetUnsettleds()).Distinct()];
            List<IKPath> ikp_l1 = [];
            int i_0 = 0;

            while (ikp_l0.Count > 0)
            {
                if (i_0 == ikp_l0.Count)
                    break;

                i_0 = ikp_l0.Count;
                List<IKPath> ikp_l2 = [];

                foreach (IKPath ikp_0 in ikp_l0)
                {
                    if (ikp_0.Root.Ancestors.Count > 0)
                        if (ikp_0.Root.Ancestors.Any(pn_l1.Contains))
                            continue;

                    if (ikp_0.EndEffect is not null && pn_l1.Contains(ikp_0.EndEffect))
                        continue;

                    PartNode[] pn_a0 = ikp_0.GetUnsettleds();

                    if (pn_a0.All(pn_l1.Contains))
                    {
                        ikp_l2.Add(ikp_0);

                        foreach (PartNode pn_0 in pn_a0)
                        {
                            pn_l1.Remove(pn_0);
                        }
                    }
                }

                foreach (IKPath ikp_0 in ikp_l2)
                {
                    ikp_l0.Remove(ikp_0);
                    ikp_l1.Add(ikp_0);
                }
            }

            IKPath[] paths = [.. ikp_l1];
            #endregion

            #region OverwritePndd
            // 参考元：https://qiita.com/Ninagawa123/items/4ae058d819de1d5b698a
            foreach (IKPath ikp_0 in paths)
            {
                PartNodeDrawDescription pndd_4r_0 = ikp_0.Root.DrawDescByOnlyEffects;
                Vector3 vec_4r2j_0 = ikp_0.Joint.DrawDescByOnlyEffects.Draw - pndd_4r_0.Draw;
                Vector3 vec_4r2j_1 = new(vec_4r2j_0.X, vec_4r2j_0.Y, 0.0f);
                Vector3 vec_4j2p_0 = ikp_0.PreEndEffect.DrawDescByOnlyEffects.Draw - ikp_0.Joint.DrawDescByOnlyEffects.Draw;
                Vector3 vec_4j2p_1 = new(vec_4j2p_0.X, vec_4j2p_0.Y, 0.0f);
                PartNode pn_4p_0 = ikp_0.PreEndEffect;
                PGoPN_InverseKinematics ikg_0p_0 = pn_4p_0.Params.InverseKinematicsGroup;
                Vector3 vec_4p2e_0 = new(
                        (float)ikg_0p_0.PreEndEffectorPointX,
                        (float)ikg_0p_0.PreEndEffectorPointY,
                        .0f);
                string s_4peepn0 = ikg_0p_0.PreEndEffectorPointName;

                if (!string.IsNullOrEmpty(s_4peepn0) &&
                    pn_4p_0.Params.CustomPointGroup.CustomNamePoints.FirstOrDefault(p => p.Name == s_4peepn0)
                    is CustomNamePointParam cnp_0)
                {
                    vec_4p2e_0 += new Vector3(
                        (float)cnp_0.X,
                        (float)cnp_0.Y,
                        0.0f);
                }

                Vector3 vec_4cstp0e_0 = new(
                    (float)ikg_0p_0.EndEffectorPointX,
                    (float)ikg_0p_0.EndEffectorPointY,
                    0.0f);
                Vector3 vec_4r2e_0;

                if (ikp_0.EndEffect is PartNode pn_4e_0)
                {
                    PartNodeDrawDescription pndd_4e_0 = pn_4e_0.DrawDescByOnlyEffects;
                    Vector2 vec_4zm0e_0 = pndd_4e_0.Zoom;

                    if (pndd_4e_0.Inverse) vec_4zm0e_0.X *= -1.0f;

                    vec_4r2e_0 = pndd_4e_0.Draw - pndd_4r_0.Draw + Vector3.Transform(
                        vec_4cstp0e_0 * new Vector3(vec_4zm0e_0, 0.0f),
                        pndd_4e_0.Rotate);
                    vec_4r2e_0.Z = 0.0f;
                }
                else
                {
                    vec_4r2e_0 = vec_4cstp0e_0 - pndd_4r_0.Draw;
                }

                PartNodeDrawDescription pndd_4p_0 = pn_4p_0.DrawDescByOnlyEffects;
                Vector2 vec_4zm0p_0 = pndd_4p_0.Zoom;

                if (pndd_4p_0.Inverse) vec_4zm0p_0.X *= -1.0f;

                Vector3 vec_4p2e_1 = Vector3.Transform(
                    vec_4p2e_0 * new Vector3(vec_4zm0p_0, 0.0f),
                    pndd_4p_0.Rotate);
                Vector3 vec_4j2e_0 = vec_4j2p_0 + vec_4p2e_1;
                float d_4L0 = vec_4r2e_0.Length();
                float d_4L1 = vec_4r2j_0.Length();
                float d_4L2 = vec_4j2e_0.Length();
                float d_4alpha = MathF.Acos((d_4L1 * d_4L1 + d_4L2 * d_4L2 - d_4L0 * d_4L0) / 2 * d_4L1 * d_4L2);
                float d_4beta = MathF.Acos((d_4L1 * d_4L1 - d_4L2 * d_4L2 + d_4L0 * d_4L0) / (2.0f * d_4L1 * d_4L0));
                float d_4gamma = MathF.Atan2(vec_4r2e_0.Y, vec_4r2e_0.X);
                float d_4theta1_0 = d_4gamma - d_4beta;
                float d_4theta1_1 = d_4gamma + d_4beta;
                float d_4theta2_0 = MathF.PI - d_4alpha;
                Vector3 vec_4r2j_2 = new(d_4L1 * MathF.Cos(d_4theta1_0), d_4L1 * MathF.Sin(d_4theta1_0), 0.0f);
                Vector3 vec_4r2j_3 = new(d_4L1 * MathF.Cos(d_4theta1_1), d_4L1 * MathF.Sin(d_4theta1_1), 0.0f);
                Vector3 vec_4r2j_4;
                float d_4theta1_2;

                if ((vec_4r2j_2 - vec_4r2j_1).Length() < (vec_4r2j_3 - vec_4r2j_1).Length())
                {
                    d_4theta1_2 = d_4theta1_0;
                    vec_4r2j_4 = vec_4r2j_2;
                }
                else
                {
                    d_4theta1_2 = d_4theta1_1;
                    vec_4r2j_4 = vec_4r2j_3;
                    d_4theta2_0 *= -1.0f;
                }

                float d_4dth1_0 = d_4theta1_2 - MathF.Atan2(vec_4r2j_1.Y, vec_4r2j_1.X);
                float d_4dth2_0 = d_4theta1_2 + d_4theta2_0 - MathF.Atan2(vec_4j2e_0.Y, vec_4j2e_0.X);
                PartNodeDrawDescription pndd_4j_0 = ikp_0.Joint.DrawDescByOnlyEffects;
                Matrix4x4 m_4rtt4j2p = Matrix4x4.CreateRotationZ(d_4dth2_0);
                PartNodeDrawDescription pndd_4j_1 = pndd_4j_0 with
                {
                    Draw = pndd_4r_0.Draw + vec_4r2j_4,
                    Rotate = pndd_4j_0.Rotate * m_4rtt4j2p
                };
                ikp_0.Joint.DrawDescByIK = pndd_4j_1;
                Matrix4x4 m_4rtt4r2j_0 = Matrix4x4.CreateRotationZ(d_4dth1_0);
                ikp_0.Root.DrawDescByIK = pndd_4r_0 with
                {
                    Rotate = pndd_4r_0.Rotate * m_4rtt4r2j_0
                };

                foreach (PartNode pn_0 in ikp_0.R2J)
                {
                    PartNodeDrawDescription pndd_0 = pn_0.DrawDescByOnlyEffects;
                    Vector3 vec_0 = pndd_0.Draw;
                    Vector3 vec_1 = Vector3.Transform(vec_0 - pndd_4r_0.Draw, m_4rtt4r2j_0);
                    PartNodeDrawDescription pndd_1 = pndd_0 with
                    {
                        Draw = pndd_4r_0.Draw + vec_1,
                        Rotate = pndd_0.Rotate * m_4rtt4r2j_0
                    };
                    pn_0.DrawDescByIK = pndd_1;
                }

                PartNode[] pn_a_4j2p_0 = [.. ikp_0.J2P, ikp_0.PreEndEffect];

                foreach (PartNode pn_0 in pn_a_4j2p_0)
                {
                    PartNodeDrawDescription pndd_0 = pn_0.DrawDescByOnlyEffects;
                    Vector3 vec_0 = pndd_0.Draw;
                    Vector3 vec_1 = Vector3.Transform(vec_0 - pndd_4j_0.Draw, m_4rtt4j2p);
                    PartNodeDrawDescription pndd_1 = pndd_0 with
                    {
                        Draw = pndd_4j_1.Draw + vec_1,
                        Rotate = pndd_0.Rotate * m_4rtt4j2p
                    };
                    pn_0.DrawDescByIK = pndd_1;
                }
            }
            #endregion
        }
    }
}
