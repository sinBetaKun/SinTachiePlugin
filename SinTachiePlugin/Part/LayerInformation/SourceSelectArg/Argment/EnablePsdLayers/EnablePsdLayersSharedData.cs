using System.Collections.Immutable;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.EnablePsdLayers
{
    internal class EnablePsdLayersSharedData
    {
        public ImmutableList<string> EnablePsdLayers { get; set; } = [];

        public EnablePsdLayersSharedData()
        {
        }

        public EnablePsdLayersSharedData(IEnablePsdLayersParameter parameter)
        {
            EnablePsdLayers = parameter.EnablePsdLayers;
        }

        public void CopyTo(IEnablePsdLayersParameter parameter)
        {
            parameter.EnablePsdLayers = EnablePsdLayers;
        }
    }
}
