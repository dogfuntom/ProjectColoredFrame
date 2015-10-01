namespace ProjectColoredFrame
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;
    using Microsoft.VisualStudio.Settings;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Settings;
    using static Global;

    [ClassInterface(ClassInterfaceType.AutoDual)]
    [CLSCompliant(false), ComVisible(true)]
    [Guid(ProjectColoredFramePackageGuids.PackageGuidString)]
    public class ProjectColoredFrameOptionsGrid : DialogPage
    {
        private const string CollectionName = @"ProjectColoredFrame";

        private const string OpacityAndThicknessCategory = @"Opacity & thickness";
        private const string CustomPaletteCategory = @"Custom palette";

        private const string CustomColorsPropertyName = nameof(CustomColors);

        private const string Separator = @"~";

        private const byte opacityDefault = byte.MaxValue / 5;
        private const byte thicknessDefault = 2;

        private const bool replaceDefaultPaletteDefault = false;

        [Category(OpacityAndThicknessCategory)]
        [DisplayName("Opacity")]
        [Description("Opacity of colored border. Value between 0 and 255.")]
        [DefaultValue(opacityDefault)]
        public byte Opacity
        { get; set; } = opacityDefault;

        [Category(OpacityAndThicknessCategory)]
        [DisplayName("Thickness")]
        [Description("Thickness of colored border in pixels. Value between 0 and 255.")]
        [DefaultValue(thicknessDefault)]
        public byte Thickness
        { get; set; } = thicknessDefault;

        [Category(CustomPaletteCategory)]
        [DisplayName("Replace default palette")]
        [Description("Replace default color set instead of appending to it. If custom palette is empty, this option will be ignored.")]
        [DefaultValue(replaceDefaultPaletteDefault)]
        public bool ReplaceDefaultPalette
        { get; set; } = replaceDefaultPaletteDefault;

        [Category(CustomPaletteCategory)]
        [DisplayName("Additional colors")]
        [Description("Additional custom colors.")]
        public Color[] CustomColors
        {
            get; set;
        } = new Color[0];

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);
            RaiseSettingsChanged(this);
        }

        public override void SaveSettingsToStorage()
        {
            base.SaveSettingsToStorage();

            var userSettingsStore = GetUserSettingsStore();
            if (!userSettingsStore.CollectionExists(CollectionName))
                userSettingsStore.CreateCollection(CollectionName);

            var converter = new ColorConverter();
            var convertedColors = new List<string>();
            foreach (var color in this.CustomColors)
            {
                convertedColors.Add(converter.ConvertToString(color));
            }

            userSettingsStore.SetString(
                CollectionName,
                CustomColorsPropertyName,
                string.Join(Separator, convertedColors));
        }

        public override void LoadSettingsFromStorage()
        {
            base.LoadSettingsFromStorage();

            var userSettingsStore = GetUserSettingsStore();
            if (!userSettingsStore.CollectionExists(CollectionName))
                return;

            var colorsString = userSettingsStore.GetString(
                CollectionName,
                CustomColorsPropertyName,
                string.Empty);

            var converter = new ColorConverter();
            var colors = new List<Color>();
            foreach (var colorString in colorsString.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries))
            {
                colors.Add((Color)converter.ConvertFromString(colorsString));
            }

            this.CustomColors = colors.ToArray();
        }

        private static WritableSettingsStore GetUserSettingsStore()
        {
            var settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            var userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            return userSettingsStore;
        }
    }
}
