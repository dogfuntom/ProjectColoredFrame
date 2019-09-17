// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace ProjectColoredFrame.Settings
{
    /// <summary>
    /// Intended to be used by this folder's code via <c>using static</c>, hence the name.
    /// </summary>
    internal static class OptionsStatic
    {
		public const string CollectionName = Global.Name;
		public const string FullCollectionName = CollectionName;

		public const string CustomPalettePropertyName = "CustomColors";

        private static readonly Lazy<IReadOnlyList<Color>> s_predefinedPalette = new Lazy<IReadOnlyList<Color>>(() => new ReadOnlyCollection<Color>(new[] {
			(Color)ColorConverter.ConvertFromString("#FFE51400"), // red
            (Color)ColorConverter.ConvertFromString("#FF60A917"), // green
            (Color)ColorConverter.ConvertFromString("#FFE3C800"), // yellow
            (Color)ColorConverter.ConvertFromString("#FF0050EF"), // cobalt
            (Color)ColorConverter.ConvertFromString("#FFD80073"), // magenta
            (Color)ColorConverter.ConvertFromString("#FF00ABA9")  // teal
        }));

        public static IReadOnlyList<Color> PredefinedPalette => s_predefinedPalette.Value;

		public static WritableSettingsStore GetUserSettingsStore()
		{
            ThreadHelper.ThrowIfNotOnUIThread();
            var settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
			var userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
			return userSettingsStore;
		}

        public static MemoryStream TryGetPropertyStream(string property)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
			var userSettingsStore = GetUserSettingsStore();
			if (!userSettingsStore.CollectionExists(FullCollectionName))
				return null;

            // Read custom colors.
            if (!userSettingsStore.PropertyExists(FullCollectionName, property))
                return null;

            return userSettingsStore.GetMemoryStream(FullCollectionName, property);
        }
    }
}
