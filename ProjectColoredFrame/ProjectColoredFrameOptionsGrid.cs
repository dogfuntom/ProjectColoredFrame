﻿namespace ProjectColoredFrame
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
    using System.IO;

    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    [Guid(ProjectColoredFramePackageGuids.PackageGuidString)]
    public class ProjectColoredFrameOptionsGrid : DialogPage
    {
        private const string CollectionName = Global.Name;

        private readonly string FullCollectionName =
            //@"ApplicationPrivateSettings\" + CollectionName + @"\" + nameof(ProjectColoredFrameOptionsGrid);
            CollectionName;

        private const string OpacityAndThicknessCategory = @"Opacity & thickness";
        private const string CustomPaletteCategory = @"Custom palette";

        private const string CustomColorsPropertyName = nameof(CustomColors);

        private const string Separator = @"~";

        private const byte opacityDefault = byte.MaxValue / 4;
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
            Global.Package.SettingsChangedEventDispatcher.RaiseSettingsChanged(this);
        }

        public override void SaveSettingsToStorage()
        {
            base.SaveSettingsToStorage();

            var userSettingsStore = GetUserSettingsStore();
            if (!userSettingsStore.CollectionExists(FullCollectionName))
                userSettingsStore.CreateCollection(FullCollectionName);

            //var converter = new ColorConverter();
            //var convertedColors = new List<string>();
            //foreach (var color in this.CustomColors)
            //{
            //    convertedColors.Add(converter.ConvertToString(color));
            //}

            //userSettingsStore.SetString(
            //    CollectionName,
            //    CustomColorsPropertyName,
            //    string.Join(Separator, convertedColors));

            using (var stream = new MemoryStream(4))
            {
                foreach (var color in this.CustomColors)
                {
                    stream.WriteByte(color.A);
                    stream.WriteByte(color.R);
                    stream.WriteByte(color.G);
                    stream.WriteByte(color.B);
                }

                stream.Seek(0, SeekOrigin.Begin);

                userSettingsStore.SetMemoryStream(FullCollectionName, CustomColorsPropertyName, stream);
            }
        }

        public override void LoadSettingsFromStorage()
        {
            base.LoadSettingsFromStorage();

            var userSettingsStore = GetUserSettingsStore();
            if (!userSettingsStore.CollectionExists(FullCollectionName))
                return;

            //var colorsString = userSettingsStore.GetString(
            //    CollectionName,
            //    CustomColorsPropertyName,
            //    string.Empty);

            //var converter = new ColorConverter();
            //var colors = new List<Color>();
            //foreach (var colorString in colorsString.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries))
            //{
            //    colors.Add((Color)converter.ConvertFromString(colorString));
            //}

            //this.CustomColors = colors.ToArray();

            using (var stream = userSettingsStore.GetMemoryStream(FullCollectionName, CustomColorsPropertyName))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var length = (int)(stream.Length / 4);
                var colors = new List<Color>(length);

                for (var i = 0; i < length; i++)
                {
                    colors.Add(Color.FromArgb(stream.ReadByte(), stream.ReadByte(), stream.ReadByte(), stream.ReadByte()));
                }

                this.CustomColors = colors.ToArray();
            }
        }

        private static WritableSettingsStore GetUserSettingsStore()
        {
            var settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            var userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            return userSettingsStore;
        }
    }
}
