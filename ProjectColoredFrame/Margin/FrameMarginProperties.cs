// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace ProjectColoredFrame
{
    using System.Threading.Tasks;
    using System.Windows.Media;

    /// <summary>
    /// Helps FrameMargin to deal with loading and reloading appearence settings.
    /// </summary>
    internal sealed class FrameMarginProperties
    {
        private readonly string _filePath;

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
            this._filePath = filePath;
        }

        public async Task UpdateAsync()
        {
            var package = await Global.GetPackageAsync();

            var services = package.Services;
            var options = package.Settings;

            Opacity = options.Opacity;
            Thickness = options.Thickness;

            Color = !string.IsNullOrWhiteSpace(_filePath)
                ? services.Mapping.GetColorOf(_filePath)
                : Colors.Transparent;
        }
    }
}
