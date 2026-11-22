
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
        public int Depth { get; set => Set(ref field, value); } = 0;

        /// <summary>
        /// リストにおける、自分を内包するグループのインデックス
        /// </summary>
        public int ParentIndex
        {
            get;
            set => Set(ref field, value);
        } = -1;

        /// <summary>
        /// ユーザがコントローラで制御できるパラメータ群
        /// </summary>
        [Display(AutoGenerateField = true)]
        public ControlledParametersOfPart ControlledParameters { get; set => Set(ref field, value); } = new();

        [JsonIgnore]
        public Thickness DepthMargin { get; set => Set(ref field, value); } = new(0, 0, 0, 0);
        const double Coef = 20;

        protected override IEnumerable<IAnimatable> GetAnimatables() => [ControlledParameters];

        public PartValue()
        {
        }

        /// <summary>
        /// 複製用コンストラクタ。
        /// </summary>
        /// <param name="origin">複製元</param>
        public PartValue(PartValue origin)
        {
            Depth = origin.Depth;
            ParentIndex = origin.ParentIndex;
            ControlledParameters = new (origin.ControlledParameters);
        }

        [Obsolete]
        public PartValue(PartBlock block)
        {
            Depth = 0;
            ParentIndex = -1;
            ControlledParameters = new(block);
        }

        public override ValueTask EndEditAsync()
        {
            DepthMargin = new Thickness(Depth * Coef, 0, 0, 0);
            return base.EndEditAsync();
        }
    }
}
