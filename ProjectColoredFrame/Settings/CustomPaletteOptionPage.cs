// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Color = System.Drawing.Color;

namespace ProjectColoredFrame.Settings
{
    using static OptionsStatic;

    // See https://blog.danskingdom.com/adding-a-wpf-settings-page-to-the-tools-options-dialog-window-for-your-visual-studio-extension/
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("8A063165-1DA7-4E49-AE91-8F3EED1E0330")]
    [System.Diagnostics.DebuggerDisplay("Count={_customColors.Length}, Replace Default={_replacePalette}")]
    public sealed class CustomPaletteOptionPage : UIElementDialogPage
    {
        #region Overridden Members
        protected override UIElement Child => new CustomPaletteEditor(_context);

        // "Should be overridden to reset settings to their default values." is said at the docs.microsoft.
        public override void ResetSettings()
        {
            base.ResetSettings();
            IncludeBuiltInPalette = true;
            _customColors = Array.Empty<Color>();
        }
        #endregion

        private readonly CustomPaletteContext _context = new CustomPaletteContext();

        private Color[] _customColors = Array.Empty<Color>();

        public IReadOnlyList<Color> CustomPalette => _customColors;

        public bool IncludeBuiltInPalette { get; set; }

        public override void LoadSettingsFromStorage()
        {
            base.LoadSettingsFromStorage();

            // Custom Palette.
            using (var stream = TryGetPropertyStream(CustomPalettePropertyName))
            {
                if (stream != null)
                {
                    _ = stream.Seek(0, SeekOrigin.Begin);
                    var length = (int)(stream.Length / 4);
                    var colors = new List<Color>(length);

                    for (var i = 0; i < length; i++)
                    {
                        colors.Add(Color.FromArgb(stream.ReadByte(), stream.ReadByte(), stream.ReadByte(), stream.ReadByte()));
                    }

                    _customColors = colors.ToArray();
                }
            }

            ApplyToContext();
        }

        // NOTE: See readme.txt in Settings folder.
        public override void SaveSettingsToStorage()
        {
            ApplyFromContext();
            base.SaveSettingsToStorage();

            ThreadHelper.ThrowIfNotOnUIThread();
            var userSettingsStore = GetUserSettingsStore();
            if (!userSettingsStore.CollectionExists(FullCollectionName))
                userSettingsStore.CreateCollection(FullCollectionName);

            // Save custom colors.
            using (var stream = new MemoryStream(4))
            {
                foreach (var color in _customColors)
                {
                    stream.WriteByte(color.A);
                    stream.WriteByte(color.R);
                    stream.WriteByte(color.G);
                    stream.WriteByte(color.B);
                }

                _ = stream.Seek(0, SeekOrigin.Begin);

                userSettingsStore.SetMemoryStream(FullCollectionName, CustomPalettePropertyName, stream);
            }
        }

        private void ApplyFromContext()
        {
            _customColors = (from color in _context.CustomColors
                             select color.ToWindowsForms()).ToArray();

            IncludeBuiltInPalette = _context.IncludeBuiltInPalette;
        }

        private void ApplyToContext()
        {
            _context.CustomColors.Clear();
            foreach (var color in _customColors)
            {
                _context.CustomColors.Add(color.ToWpf());
            }

            _context.IncludeBuiltInPalette = IncludeBuiltInPalette;
        }
    }
}
