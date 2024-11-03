using SinTachiePlugin.Parts;
using SinTachiePlugin.ShapePludin.PartsListControllerForShape;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        //[Display(Name = "素材の場所", Description = "立ち絵画像を保存しているフォルダ\n（ここを変更するだけでは「描画順序」には反映されませんので、「ルート」にパスをコピペしてください。）")]
        //[DirectorySelector(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        //public string Directory { get => directory; set => Set(ref directory, value); }
        //string directory = string.Empty;

        [Display(Name = "描画順序", Description = "描画順序\n(アイテムを閉じると「ルート」が空になってしまうので、その都度「素材の場所」のパスをコピペして使ってください。)")]
        [PartsListControllerForShape(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public PartsOfShapeItem PartsAndRoot { get => partsAndRoot; set => Set(ref partsAndRoot, value); }
        PartsOfShapeItem partsAndRoot = new();

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

        protected override IEnumerable<IAnimatable> GetAnimatables() => PartsAndRoot.Parts;

        protected override void LoadSharedData(SharedDataStore store)
        {
            var data = store.Load<SharedData>();
            if (data is null)
                return;
            data.CopyTo(this);
        }

        protected override void SaveSharedData(SharedDataStore store)
        {
            throw new NotImplementedException();
        }

        public class SharedData(ShapeParameterOfSinTachie parameter)
        {
            public string Directory { get; set; } = parameter.PartsAndRoot.Root;
            public ImmutableList<PartBlock> Parts { get; } = [.. parameter.PartsAndRoot.Parts.Select(x => new PartBlock(x))];

            public void CopyTo(ShapeParameterOfSinTachie parameter)
            {
                var newParam = new PartsOfShapeItem() { Root = Directory, Parts = [.. Parts.Select(x => new PartBlock(x))] };
                parameter.PartsAndRoot = newParam;
            }
        }
    }
}
