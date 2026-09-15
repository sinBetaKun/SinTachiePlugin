using SinTachiePlugin.CharacterAsset;
using SinTachiePlugin.Control.PartValueList;
using SinTachiePlugin.Part;
using SinTachiePlugin.ShapePludin.PartsListControllerForShape;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.ShapePludin
{
    internal class ShapeParameterOfSinTachie(SharedDataStore? sharedData) : ShapeParameterBase(sharedData)
    {
        [Display]
        [PartValueListForShape(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public PartValuesAndAssets PartValuesAndAssets { get => _partValues; set => Set(ref _partValues, value); }
        private PartValuesAndAssets _partValues = new();

        [Obsolete]
        public PartsOfShapeItem PartsAndRoot
        {
            set
            {
                CharacterAssetsData ad_0 = new();
                ad_0.ChangePath(value.Root);
                PartValuesAndAssets = new PartValuesAndAssets()
                {
                    PartValues = [.. value.Parts.Select(p => new PartValue(p))],
                    Assets = [ad_0],
                };
            }
        }


        public ShapeParameterOfSinTachie() : this(null)
        {
        }

        public override IEnumerable<string> CreateMaskExoFilter(int keyFrameIndex, ExoOutputDescription desc, ShapeMaskExoOutputDescription shapeMaskParameters)
        {
            return [];
        }

        public override IEnumerable<string> CreateShapeItemExoFilter(int keyFrameIndex, ExoOutputDescription desc)
        {
            return [];
        }

        public override IShapeSource CreateShapeSource(IGraphicsDevicesAndContext devices)
        {
            return new ShapeOfSinTachieSource(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [.. PartValuesAndAssets.PartValues];

        protected override void LoadSharedData(SharedDataStore store)
        {
            var data = store.Load<SharedData>();
            if (data is null)
                return;
            data.CopyTo(this);
        }

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new SharedData(this));
        }

        public class SharedData(ShapeParameterOfSinTachie parameter)
        {
            public PartValuesAndAssets PartValuesAndAssets { get; } = new()
            {
                Assets = [.. parameter.PartValuesAndAssets.Assets.Select(x => new CharacterAssetsData(x))],
                PartValues = [.. parameter.PartValuesAndAssets.PartValues.Select(x => new PartValue(x))]
            };

            public void CopyTo(ShapeParameterOfSinTachie parameter)
            {
                parameter.PartValuesAndAssets = PartValuesAndAssets;
            }
        }
    }
}
