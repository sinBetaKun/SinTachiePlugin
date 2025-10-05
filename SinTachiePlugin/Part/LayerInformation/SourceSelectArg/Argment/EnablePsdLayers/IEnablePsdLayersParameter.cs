using System.Collections.Immutable;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.EnablePsdLayers
{
    internal interface IEnablePsdLayersParameter
    {
        public ImmutableList<string> EnablePsdLayers { get; set; }
    }
}
