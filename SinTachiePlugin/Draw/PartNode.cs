using SinTachiePlugin.Draw.EffectChain;
using SinTachiePlugin.Draw.OverallManaging;
using SinTachiePlugin.Draw.ParamGroupOfPartNode;
using SinTachiePlugin.Draw.PointParam;
using SinTachiePlugin.EffectChainFlag;
using SinTachiePlugin.Enums;
using SinTachiePlugin.PartAnimation.Node.ImageFileNode;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg;
using System.Collections.Immutable;
using System.IO;
using System.Numerics;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace SinTachiePlugin.Draw
{
    internal class PartNode : IDisposable
    {
        public string Tag { get; set; } = string.Empty;
        public ParamsOfPartNode Params { get; init; }
        public PartNodeComparateResult PrevCompResult { get; private set; } = new();
        public ImmutableList<PartNode> Ancestors { get; set; } = []; // 祖先の果ては末尾
        private ImmutableList<VideoEffectChainNode> _effectCain = [];
        public ImmutableList<PartNode> ClippingPath { get; private set; } = [];

        private ImageFileNodeManager? _ifnm;
        private readonly IGraphicsDevicesAndContext _devices;
        private readonly ID2D1Bitmap _empty;
        private ID2D1Image? _source; // SourceManagerで管理しているため、ノード内部でDisposeしてはいけない
        private ID2D1Image _preOutput; // ただの一次沖であるため、Disposeすると他のDispose処理と重複するため厳禁
        public PartNodeDrawDescription DrawDescByOnlyEffects { get; private set; } = PartNodeDrawDescription.Identity;
        public PartNodeDrawDescription? DrawDescByIK { get; set; } = PartNodeDrawDescription.Identity;
        private readonly PartNodeDrawer _drawer;
        public ID2D1Image Output => _drawer.Output;
        public float Z { get; private set; } = 0.0f;
        public bool OutputChanged { get; private set; } = true;
        public bool ZChanged { get; private set; } = true;
        //private bool _sourceChanged = false;

        public PartNode(IGraphicsDevicesAndContext devices, AudioSourceManager asm)
        {
            Params = new(asm);
            _devices = devices;
            _empty = devices.DeviceContext.CreateEmptyBitmap();
            _preOutput = _empty;
            _drawer = new(devices);
        }

        public void SetSource(ID2D1Image source)
        {
            _source = source;
            //if (_source != source)
            //{
            //    _source = source;
            //    _sourceChanged = true;
            //}
            //else
            //{
            //    _sourceChanged = false;
            //}
        }

        /// <summary>
        /// PartNodeが持つImageFileNodeManagerを更新し、
        /// 多次元ｱﾆﾒｰｼｮﾝﾚｲﾔｰの出力値から一枚のｱﾆﾒｰｼｮﾝ差分を選出した状態にする。
        /// </summary>
        /// <param name="results">第一次出力値リスト</param>
        /// <param name="nModes">正規化モードリスト（絶対にresultsに順番に対応させる）</param>
        public void UpdateIfnm(List<PartAnimationResultA> results, List<PartAnimationNormalizationMode> nModes)
        {
            if (!(_ifnm is not null && _ifnm.Root?.Path == Params.SourceGroup.FilePath))
            {
                _ifnm = new(Params.SourceGroup.FilePath);
            }

            _ifnm.Update(results, nModes);
        }

        public void ClearIfnm()
        {
            _ifnm = null;
        }

        public string GetSourcePath()
        {
            if (_ifnm is not null && File.Exists(_ifnm.Value))
            {
                return _ifnm.Value;
            }
            else
            {
                return Params.SourceGroup.FilePath;
            }
        }

        public void UpdateParams(List<PartValueAndFL> pvfls, TachieSourceDescription desc)
        {
            if (pvfls.Count == 0)
            {
                PrevCompResult = new(Exception: true);
                return;
            }

            Tag = pvfls.First().PartValue.ControlledParameters.Tag;
            Params.Update(pvfls, desc);
            PrevCompResult = Params.ComparateWithPrev();
            Params.SyncPrev();
        }

        public void UpdatePreOutput(TimelineSourceDescription tsDesc)
        {
            ID2D1Image img_0 = _source ?? _empty;
            List<VideoEffectChainNode> vecn_l0 = [.. _effectCain];
            List<VideoEffectChainNode> vecn_l1 = [];
            PartNode[] pn_a0 = [this, .. Ancestors];
            VideoEffectChainNode vecn_0;
            List<PartEffectsAndFL> pefl_l0 = [];
            List<IVideoEffect> ve_l0 = [];
            bool b_0 = true; // 現在処理しているのが先祖(f)なのか自身(t)なのか
            List<PartNodeDrawDescription> pndd_l0 = [];
            List<PartNode> pn_l0 = [];
            float d_4opc_0 = 1.0f;
            Vector2 vec_4zm_0 = new(1, 1);
            InterpolationMode intm_0 = InterpolationMode.HighQualityCubic;
            Matrix4x4 m_4rtt_0 = Matrix4x4.Identity;
            bool b_4inv_0 = false;
            Matrix4x4 m_4cmr_0 = Matrix4x4.Identity;
            PGoPN_ValueDependent vdg_0 = new();

            // 自身から祖先へ順番に
            foreach (PartNode pn_0 in pn_a0)
            {
                #region GenerateEffects
                if (vecn_l0.FirstOrDefault(x => x.Pn == pn_0) is VideoEffectChainNode vecn_1)
                {
                    vecn_l0.Remove(vecn_1);
                    vecn_l1.Add(vecn_1);
                    vecn_0 = vecn_1;
                }
                else
                {
                    vecn_0 = new(_devices, pn_0);
                    vecn_l1.Add(vecn_0);
                }

                PGoPN_CenterPoint ctpg_0 = pn_0.Params.CenterPointGroup;
                PGoPN_Drawing dg_0 = pn_0.Params.DrawingGroup;
                Vector3 vct_0 = new((float)dg_0.X, (float)dg_0.Y, (float)dg_0.Z);
                Vector2 vct_1;

                if (ctpg_0.CenterMode != CenterPointMode.DontSet)
                {
                    float x = (float)ctpg_0.X;
                    float y = (float)ctpg_0.Y;

                    if (ctpg_0.CenterMode == CenterPointMode.CustomPointName)
                    {
                        PGoPN_CustomPoint cspg0 = pn_0.Params.CustomPointGroup;

                        if (cspg0.CustomNamePoints.FirstOrDefault(p => p.Name == ctpg_0.CustomPointName) is CustomNamePointParam p_0)
                        {
                            x += (float)p_0.X;
                            y += (float)p_0.Y;
                        }
                    }

                    vct_1 = new Vector2(x, y);

                    if (ctpg_0.KeepPlace)
                    {
                        vct_0 += new Vector3(vct_1, 0);
                    }
                }
                else
                {
                    vct_1 = new();
                }

                DrawDescription dd_0 = new(
                    vct_0,
                    default,
                    new Vector2((float)dg_0.Zoom_X, (float)dg_0.Zoom_Y),
                    new Vector3(0, 0, (float)dg_0.Rotate),
                    Matrix4x4.Identity,
                    InterpolationMode.Linear,
                    (float)dg_0.Opacity,
                    dg_0.Invert,
                    []
                    );

                pefl_l0.Clear();

                // 自身にかかるエフェクトは、フラグを無視する
                if (b_0)
                {
                    foreach (PartEffectsAndFL pefl_0 in pn_0.Params.PartEffectGroup.Effects)
                    {
                        ve_l0.Clear();

                        foreach (IVideoEffect ve_0 in pefl_0.Effects)
                            if (ve_0.IsEnabled)
                                ve_l0.Add(ve_0);

                        pefl_l0.Add(new(ve_l0, pefl_0.FL));
                    }
                }
                // 先祖にかかるエフェクトはフラグで有効無効を切り替える
                else
                {
                    // フラグ用
                    bool b_1 = true;

                    foreach (PartEffectsAndFL pefl_0 in pn_0.Params.PartEffectGroup.Effects)
                    {
                        ve_l0.Clear();

                        foreach (IVideoEffect ve_0 in pefl_0.Effects)
                        {
                            if (ve_0.IsEnabled)
                            {
                                if (ve_0 is SinTachiePlugin_PartEffectChainFlag pecf_0)
                                {
                                    b_1 = pecf_0.Flag;
                                }
                                else if (b_1)
                                {
                                    ve_l0.Add(ve_0);
                                }
                            }
                        }

                        pefl_l0.Add(new(ve_l0, pefl_0.FL));
                    }
                }

                b_0 = false;
                #endregion

                #region CarcLazyEffect
                DrawDescription dd_1 = vecn_0.Update(pefl_l0, img_0, tsDesc, dd_0);

                if (vdg_0.UnlazyEffect)
                    img_0 = vecn_0.Output;

                Matrix4x4 m_4rtt_2 = Matrix4x4.CreateRotationZ(MathF.PI * dd_0.Rotation.Z / 180f)
                    * Matrix4x4.CreateRotationY(MathF.PI * -dd_0.Rotation.Y / 180f)
                    * Matrix4x4.CreateRotationX(MathF.PI * -dd_0.Rotation.X / 180f);
                PGoPN_ValueDependent vdg_1 = pn_0.Params.ValueDependentGroup;

                if (vdg_0.XYZ)
                {
                    pndd_l0.Add(new(
                        dd_0.Draw,
                        vct_1,
                        dd_0.Zoom,
                        m_4rtt_2,
                        dd_0.Camera,
                        0,
                        dd_0.Invert,
                        InterpolationMode.HighQualityCubic));
                    pn_l0.Add(pn_0);
                }

                if (vdg_0.Zoom)
                {
                    vec_4zm_0 *= dd_0.Zoom;
                    intm_0 = dd_0.ZoomInterpolationMode;
                }

                if (vdg_0.Rotation)
                    m_4rtt_0 *= m_4rtt_2;

                if (vdg_0.Camera)
                    m_4cmr_0 *= dd_0.Camera;

                if (vdg_0.Opacity)
                    d_4opc_0 *= (float)dd_0.Opacity;

                if (vdg_0.Invert)
                    b_4inv_0 ^= dd_0.Invert;
                #endregion

                #region ValueDependent
                vdg_0.XYZ &= vdg_1.XYZ;
                vdg_0.Opacity &= vdg_1.Opacity;
                vdg_0.Zoom &= vdg_1.Zoom;
                vdg_0.Rotation &= vdg_1.Rotation;
                vdg_0.Invert &= vdg_1.Invert;
                vdg_0.Camera &= vdg_1.Camera;
                vdg_0.UnlazyEffect &= vdg_1.UnlazyEffect;
                #endregion
            }

            vecn_l0.ForEach(x => x.Dispose());
            _effectCain = [.. vecn_l1];

            Vector2 vec_4zm_1 = new(1.0f, 1.0f);
            Matrix4x4 m_4rtt_1 = Matrix4x4.Identity;
            bool b_4inv_1 = false;
            Vector3 vec_4dr_0 = new();
            Vector3 vec_0;
            List<CustomNamePointParam> cnp_l0 = [];
            pndd_l0.Reverse();
            pn_l0.Reverse();

            for (int i = 0; i < pndd_l0.Count; i++)
            {
                PartNodeDrawDescription pndd_0 = pndd_l0[i];
                PartNode pn_0 = pn_l0[i];
                string s_4org_0 = pn_0.Params.OriginGroup.PointName;

                if (!string.IsNullOrEmpty(s_4org_0)
                    && cnp_l0.FirstOrDefault(p => p.Name == s_4org_0) is CustomNamePointParam cnp_0)
                {
                    vec_0 = new((float)cnp_0.X, (float)cnp_0.Y, 0);
                }
                else
                {
                    vec_0 = new();
                }

                Matrix4x4 m_0 =
                    Matrix4x4.CreateTranslation(pndd_0.Draw + vec_0)
                    * Matrix4x4.CreateScale(new Vector3(vec_4zm_1, 0))
                    * (b_4inv_1 ? Matrix4x4.CreateScale(-1f, 1f, 1f) : Matrix4x4.Identity)
                    * m_4rtt_1;

                vec_4dr_0 += new Vector3(m_0.M41, m_0.M42, m_0.M43);
                PGoPN_ValueDependent vdg_1 = pn_0.Params.ValueDependentGroup;

                if (vdg_1.Zoom)  vec_4zm_1 *= pndd_0.Zoom;
                else            vec_4zm_1 = pndd_0.Zoom;
                
                if (vdg_1.Invert)    b_4inv_1 ^= pndd_0.Inverse;
                else                b_4inv_1 = pndd_0.Inverse;

                if (vdg_1.Rotation)  m_4rtt_1 *= pndd_0.Rotate;
                else                m_4rtt_1 = pndd_0.Rotate;

                cnp_l0 = pn_0.Params.CustomPointGroup.CustomNamePoints;
            }

            _preOutput = img_0;
            PGoPN_CenterPoint ctpg_1 = Params.CenterPointGroup;
            Vector2 vec_4cnp_0;
            if (ctpg_1.CenterMode != CenterPointMode.DontSet)
            {
                float x = (float)ctpg_1.X;
                float y = (float)ctpg_1.Y;

                if (ctpg_1.CenterMode == CenterPointMode.CustomPointName)
                {
                    PGoPN_CustomPoint cspg0 = Params.CustomPointGroup;

                    if (cspg0.CustomNamePoints.FirstOrDefault(p => p.Name == ctpg_1.CustomPointName) is CustomNamePointParam p0)
                    {
                        x += (float)p0.X;
                        y += (float)p0.Y;
                    }
                }

                vec_4cnp_0 = new Vector2(x, y);
            }
            else
            {
                vec_4cnp_0 = new();
            }

            DrawDescByOnlyEffects = new(vec_4dr_0, vec_4cnp_0, vec_4zm_0, m_4rtt_0, m_4cmr_0, d_4opc_0, b_4inv_0, intm_0);
            DrawDescByIK = null;
        }

        public void UpdateOutputAndZ()
        {
            OutputChanged = _drawer.Update(_preOutput, DrawDescByIK ?? DrawDescByOnlyEffects);
            float z = (DrawDescByIK ?? DrawDescByOnlyEffects).GetZ();
            ZChanged = Z != z;
            Z = z;
        }

        public void Dispose()
        {
            _empty.Dispose();
            _effectCain.ForEach(x => x.Dispose());
            _drawer.Dispose();
        }
    }
}
