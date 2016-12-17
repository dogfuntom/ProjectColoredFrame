using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ProjectColoredFrame
{
    internal sealed class FrameMarginProperties
    {
        private readonly string filePath;

        public float Opacity
        { get; private set; }

        public float Thickness
        { get; private set; }

        public Color Color
        { get; private set; }

        public FrameMarginProperties(string filePath)
        {
            this.filePath = filePath;
        }

        public void Update()
        {
            var options = ProjectColoredFramePackage.Current.OptionsGrid;

            this.Opacity = options.Opacity;
            this.Thickness = options.Thickness;

            var color = !string.IsNullOrWhiteSpace(filePath)
                ? ColorChooser.GetColorFor(filePath)
                : Colors.Transparent;
        }
    }
}
