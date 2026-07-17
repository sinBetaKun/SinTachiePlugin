using SinTachiePlugin.Draw.ParamGroupOfPartNode;
using SinTachiePlugin.Enums;
using System.Collections.Immutable;

namespace SinTachiePlugin.Draw.InverseKinematics
{
    internal class IKPathManager
    {
        public ImmutableList<IKPath> Paths { get; private set; } = [];

        public void UpdatePaths(IEnumerable<PartNode> pns)
        {
            List<PartNode> l0 = [.. pns];
            List<IKPath> l1 = [];
            string[] a0 = [.. l0.Select(x => x.Tag).Where(x => !string.IsNullOrEmpty(x))];

            foreach (PartNode _0 in l0)
            {
                if (_0.Params.SourceGroup.ThisLayerType == LayerType.Group)
                    continue;

                PGoPN_InverseKinematics _1 = _0.Params.InverseKinematicsGroup;
                
                if (string.IsNullOrEmpty(_1.RootPartTag)
                    || string.IsNullOrEmpty(_1.JointPartTag)
                    || string.IsNullOrEmpty(_1.EndEffectorPartTag))
                {
                    continue;
                }

                if (_1.RootPartTag == _0.Tag
                    || _1.RootPartTag == _0.Tag
                    || _1.EndEffectorPartTag == _0.Tag)
                {
                    continue;
                }

                if (_1.RootPartTag == _1.JointPartTag
                    || _1.RootPartTag == _1.EndEffectorPartTag
                    || _1.EndEffectorPartTag == _1.JointPartTag)
                {
                    continue;
                }

                ImmutableList<PartNode> l2 = _0.Ancestors;
                string[] a1 = [.. l2.Select(x => x.Tag)];

                if (!a1.Contains(_1.RootPartTag)
                    || !a1.Contains(_1.JointPartTag)
                    || !a0.Contains(_1.EndEffectorPartTag)
                    || a1.Contains(_1.EndEffectorPartTag))
                {
                    continue;
                }

                int n0 = a1.IndexOf(_1.RootPartTag);
                int n1 = a1.IndexOf(_1.JointPartTag);

                if (n0 < n1)
                    continue;

                PartNode[] a2 = [.. l2.GetRange(n1 + 1, n0 - n1 - 1).Reverse()];
                PartNode[] a3 = [.. l2.GetRange(0, n1).Reverse()];

                l1.Add(new(l2[n0], a2, l2[n1], a3, _0, l0.First(x => x.Tag == _1.EndEffectorPartTag)));
            }

            List<PartNode> l3 = [.. l1.SelectMany(x => x.GetUnsettleds()).Distinct()];
            List<IKPath> l4 = [];
            int n2 = 0;

            while (l1.Count > 0)
            {
                if (n2 == l1.Count)
                    break;

                n2 = l1.Count;
                List<IKPath> l5 = [];

                foreach (IKPath _1 in l1)
                {
                    if (_1.Root.Ancestors.Count > 0)
                        if (_1.Root.Ancestors.Any(l3.Contains))
                            continue;

                    if (l3.Contains(_1.EndEffect))
                        continue;

                    PartNode[] a1 = _1.GetUnsettleds();

                    if (a1.All(l3.Contains))
                    {
                        l5.Add(_1);

                        foreach (PartNode _2 in a1)
                        {
                            l3.Remove(_2);
                        }
                    }
                }

                foreach (IKPath _1 in l5)
                {
                    l1.Remove(_1);
                    l4.Add(_1);
                }
            }

            Paths = [.. l4];
        }
    }
}
