using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project;

namespace ShapeOfSinTachie
{
    [PluginDetails(AuthorName = "sinβ")]
    internal class ShapeOfSinTachie : IShapePlugin
    {
        public bool IsExoShapeSupported => false;

        public bool IsExoMaskSupported => false;

        public string Name => "sin型カスタムパーツ立ち絵【図形】";

        public IShapeParameter CreateShapeParameter(SharedDataStore? sharedData)
        {
            return new ShapeParameterOfSinTachie(sharedData);
        }
    }
}
