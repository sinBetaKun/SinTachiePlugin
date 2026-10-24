using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.Drawing.DrawingArg.Parameter;
using SinTachiePlugin.Part.Drawing.DrawingArg.SubArgument.Parameter;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.Parent;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Part
{
    /// <summary>
    /// UIに表示するデータ用のパラメータを纏めたもの。
    /// レイヤータイプが変更された際のバインディングを楽にするためである。
    /// </summary>
    public class ViewParametersOfPart : Animatable
    {
        public string ParentView { get => _parentView; set => Set(ref _parentView, value); }
        private string _parentView = string.Empty;
        public string PriorityView { get => _priorityView; set => Set(ref _priorityView, value); }
        private string _priorityView = string.Empty;

        /// <summary>
        /// GetAnimatablesに渡す必要はない。
        /// </summary>
        public SourceSelectArgBase Ssa { get => _ssa; set => Set(ref _ssa, value); }
        private SourceSelectArgBase _ssa = new GroupParameter();

        public void Update(ControlledParametersOfPart cp)
        {
            if (cp.SourceSelectArg is IParentParameter parentParameter)
                ParentView = parentParameter.Parent;
            else
                ParentView = string.Empty;

            if (cp.DrawingArg is HavingSourceDrawingParameter hsdp)
            {
                switch (hsdp.ModeMaster)
                {
                    case DrawingValueModeMaster.Initialize:
                        PriorityView = "0.0";
                        break;

                    case DrawingValueModeMaster.Override:
                        if (hsdp.SubArg is MasterModeParameter mmp_0)
                        {
                            PriorityView = mmp_0.Priority.Values[0].Value.ToString("F1");
                        }
                        else
                        {
                            PriorityView = string.Empty;
                        }

                        break;

                    case DrawingValueModeMaster.Compose:
                        if (hsdp.SubArg is MasterModeParameter mmp_1)
                        {
                            PriorityView = mmp_1.Priority.Values[0].Value.ToString("+F1");
                        }
                        else
                        {
                            PriorityView = string.Empty;
                        }
                        break;

                    case DrawingValueModeMaster.Custom:
                        if (hsdp.SubArg is CustomModeParameter cmp_0)
                        {
                            if (cmp_0.PriorityMode == DrawingValueMode.Compose)
                            {
                                PriorityView = cmp_0.Priority.Values[0].Value.ToString("+F1");
                            }
                            else
                            {
                                PriorityView = cmp_0.Priority.Values[0].Value.ToString("F1");
                            }
                        }
                        else
                        {
                            PriorityView = string.Empty;
                        }
                        break;

                    case DrawingValueModeMaster.DontOverride:
                        PriorityView = "+0.0";
                        break;
                }
            }
            else
            {
                PriorityView = string.Empty;
            }

            Ssa = cp.SourceSelectArg;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];
    }
}
