using SinTachiePlugin.Informations;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.ShapePludin
{
    internal class ShapeOfSinTachie : IShapePlugin
    {
        public string Name => PluginInfo.Title + "(図形アイテム版)";

        /// <summary>
        /// 俺ちゃんの名前
        /// </summary>
        public PluginDetailsAttribute Details => new() { AuthorName = "sinβ" };

        public bool IsExoShapeSupported => false;

        public bool IsExoMaskSupported => false;
        
        public IShapeParameter CreateShapeParameter(SharedDataStore? sharedData)
        {
            return new ShapeParameterOfSinTachie(sharedData);
        }
    }
}
