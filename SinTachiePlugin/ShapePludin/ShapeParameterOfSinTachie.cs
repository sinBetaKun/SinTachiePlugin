using SinTachiePlugin.Control.PartValueList;
using SinTachiePlugin.Part;
using SinTachiePlugin.ShapePludin.PartsListControllerForShape;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.ShapePludin
{
    internal class ShapeParameterOfSinTachie(SharedDataStore? sharedData) : ShapeParameterBase(sharedData)
    {
        [Display]
        [DirectorySelector]
        public string Root
        {
            get => _root;
            set
            {
                PartValuesAndRoot.Root = value;
                Set(ref _root, value);
            }
        }
        private string _root = string.Empty;

        [Display]
        [PartValueListForShape(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public PartValuesAndRoot PartValuesAndRoot { get => _partValues; set => Set(ref _partValues, value); }
        private PartValuesAndRoot _partValues = new();

        [Obsolete]
        public PartsOfShapeItem PartsAndRoot
        {
            set
            {
                Root = value.Root;
                PartValuesAndRoot = new PartValuesAndRoot()
                {
                    Root = value.Root,
                    PartValues = [.. value.Parts.Select(p => new PartValue(p))]
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

        protected override IEnumerable<IAnimatable> GetAnimatables() => [.. PartValuesAndRoot.PartValues];

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
            public string Directory { get; set; } = parameter.Root;
            public ImmutableList<PartValue> PartValues { get; } = [.. parameter.PartValuesAndRoot.PartValues.Select(x => new PartValue(x))];

            public void CopyTo(ShapeParameterOfSinTachie parameter)
            {
                parameter.Root = Directory;
                parameter.PartValuesAndRoot = new()
                {
                    Root = Directory,
                    PartValues = [..PartValues.Select(x => new PartValue(x))]
                };
            }
        }
    }
}
