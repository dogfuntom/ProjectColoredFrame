namespace ProjectColoredFrame
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows.Media;
    using Microsoft.VisualStudio.Shell;
    using static Global;

    [ClassInterface(ClassInterfaceType.AutoDual)]
    [CLSCompliant(false), ComVisible(true)]
    [Guid(ProjectColoredFramePackageGuids.PackageGuidString)]
    public class ProjectColoredFrameOptionsGrid : DialogPage
    {
        private const byte opacityDefault = byte.MaxValue / 5;

        [Category("Opacity")]
        [DisplayName("Opacity")]
        [Description("Opacity of colored border. Value between 0 and 255.")]
        [DefaultValue(opacityDefault)]
        public byte Opacity
        { get; set; } = opacityDefault;

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);
            RaiseSettingsChanged(this);
        }
    }
}
