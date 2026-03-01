using System.Collections.Immutable;

namespace SinTachiePlugin.Draw
{
    internal class PsdQuery(string filePath, IEnumerable<string> enableLayers)
    {
        public string FilePath { get; init; } = filePath;
        public ImmutableList<string> EnableLayers { get; init; } = [.. enableLayers];
    }
}
