namespace ProjectColoredFrame
{
    using System.Windows.Media;

    /// <summary>
    /// Helps FrameMargin to deal with loading and reloading appearence settings.
    /// </summary>
    internal sealed class FrameMarginProperties
    {
        private readonly string filePath;

        public byte Opacity
        { get; private set; }

        public byte Thickness
        { get; private set; }

        public Color Color
        { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameMarginProperties"/> class.
        /// </summary>
        /// <param name="filePath">The file path, can be null.</param>
        public FrameMarginProperties(string filePath)
        {
            this.filePath = filePath;
        }

        public void Update()
        {
            var package = Global.Package;
            var options = ProjectColoredFramePackage.Current.OptionsGrid;

            this.Opacity = options.Opacity;
            this.Thickness = options.Thickness;

            this.Color = !string.IsNullOrWhiteSpace(this.filePath)
                ? package.Services.Mapping.GetColorOf(this.filePath)
                : Colors.Transparent;
        }
    }
}
