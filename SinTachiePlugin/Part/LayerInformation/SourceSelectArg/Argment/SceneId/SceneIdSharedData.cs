namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.SceneId
{
    internal class SceneIdSharedData
    {
        public Guid SceneId { get; set; }

        public SceneIdSharedData()
        {
        }

        public SceneIdSharedData(ISceneIdParameter parameter)
        {
            SceneId = parameter.SceneId;
        }

        public void CopyTo(ISceneIdParameter parameter)
        {
            parameter.SceneId = SceneId;
        }
    }
}
