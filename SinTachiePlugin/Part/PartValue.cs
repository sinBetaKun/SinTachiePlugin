
using Newtonsoft.Json;
using SinTachiePlugin.Parts;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Part
{
    public class PartValue : Animatable
    {
        /// <summary>
        /// リストを木構造にしたものにおける深度。
        /// </summary>
        [JsonIgnore]
        public int Depth { get => _depth; set => Set(ref _depth, value); }
        private int _depth = 0;

        /// <summary>
        /// リストにおける、自分を内包するグループのインデックス
        /// </summary>
        public int ParentIndex { get => _parentIndex; set => Set(ref _parentIndex, value); }
        private int _parentIndex = -1;

        /// <summary>
        /// ユーザがコントローラで制御できるパラメータ群
        /// </summary>
        [Display(AutoGenerateField = true)]
        public ControlledParametersOfPart ControlledParameters
        {
            get => _controledParameters;
            set
            {
                ControlledParametersOfPart oldCp = _controledParameters;
                if (Set(ref _controledParameters, value))
                {
                    oldCp.ValueUpdated -= UpdateViewParams;
                    value.ValueUpdated += UpdateViewParams;
                }
            }
        }
        private ControlledParametersOfPart _controledParameters = new();

        [JsonIgnore]
        public ViewParametersOfPart ViewParameters
        {
            get => _viewParameters;
            set => Set(ref _viewParameters, value);
        }
        private ViewParametersOfPart _viewParameters = new();

        [JsonIgnore]
        public Thickness DepthMargin { get => _depthMargin; set => Set(ref _depthMargin, value); }
        private Thickness _depthMargin = new(0, 0, 0, 0);
        const double Coef = 20;

        protected override IEnumerable<IAnimatable> GetAnimatables() => [ControlledParameters, ViewParameters];

        public PartValue()
        {
            ControlledParameters.ValueUpdated += UpdateViewParams;
            UpdateViewParams();
        }

        /// <summary>
        /// 複製用コンストラクタ。
        /// </summary>
        /// <param name="origin">複製元</param>
        public PartValue(PartValue origin)
        {
            ParentIndex = origin.ParentIndex;
            ControlledParameters = new (origin.ControlledParameters);
            ControlledParameters.ValueUpdated += UpdateViewParams;
            UpdateViewParams();
        }

        [Obsolete]
        public PartValue(PartBlock block)
        {
            Depth = 0;
            ParentIndex = -1;
            ControlledParameters = new(block);
            UpdateViewParams();
        }

        public void UpdateViewParams(object? sender, EventArgs events)
        {
            UpdateViewParams();
        }

        private void UpdateViewParams()
        {
            ViewParameters.Update(ControlledParameters);
            //ViewParametersOfPart vp = new();
            //vp.Update(ControlledParameters);
            //ViewParameters = vp;
        }

        public override ValueTask EndEditAsync()
        {
            DepthMargin = new Thickness(Depth * Coef, 0, 0, 0);
            return base.EndEditAsync();
        }
    }
}
