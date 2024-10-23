using SinTachiePlugin.Informations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.ShapePludin
{
    [PluginDetails(AuthorName = "sinβ")]
    internal class ShapeOfSinTachie : IShapePlugin
    {
        public bool IsExoShapeSupported => false;

        public bool IsExoMaskSupported => false;

        public string Name => PluginInfo.Title;

        public IShapeParameter CreateShapeParameter(SharedDataStore? sharedData)
        {
            return new ShapeParameterOfSinTachie(sharedData);
        }
    }
}
