namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.ContentOffset
{
    internal class ContentOffsetSharedData
    {
        public TimeSpan ContentOffset { get; set; } = TimeSpan.Zero;

        public ContentOffsetSharedData()
        {
        }

        public ContentOffsetSharedData(IContentOffsetParameter parameter)
        {
            ContentOffset = parameter.ContentOffset;
        }

        public void CopyTo(IContentOffsetParameter parameter)
        {
            parameter.ContentOffset = ContentOffset;
        }
    }
}
