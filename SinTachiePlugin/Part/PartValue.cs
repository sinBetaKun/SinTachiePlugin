using SinTachiePlugin.Parts;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
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
        public ControlledParametersOfPart ControlledParameters { get => controledParameters; set => Set(ref controledParameters, value); }
        private ControlledParametersOfPart controledParameters = new();

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
    }
}
