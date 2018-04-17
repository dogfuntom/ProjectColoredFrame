﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using MiniJSON;

namespace ProjectColoredFrame
{
	[ClassInterface(ClassInterfaceType.AutoDual)]
	[ComVisible(true)]
	[Guid(ProjectColoredFramePackageGuids.PackageGuidString)]
	public class ProjectColoredFrameOptionsGrid : DialogPage
	{
		private const string CollectionName = Global.Name;

		private readonly string _fullCollectionName = CollectionName;

		private const string OpacityAndThicknessCategory = "Opacity & thickness";
		private const string BehaviorCategory = "Behavior";
		private const string CustomPaletteCategory = "Custom palette";
		private const string CustomMappingsCategory = "Custom mappings";

#pragma warning disable CC0021 // Use nameof
		private const string CustomColorsPropertyName = "CustomColors";
		private const string CustomMappingsPropertyName = "CustomMappings";
#pragma warning restore CC0021 // Use nameof

		private const byte OpacityDefault = byte.MaxValue / 4;
		private const byte ThicknessDefault = 2;

		private const bool FactorInSolutionPathDefault = false;

		private const bool ReplaceDefaultPaletteDefault = false;
		private const string ReplaceDefaultPaletteDisplayName = "Replace default palette";

		[Category(OpacityAndThicknessCategory)]
		[DisplayName("Opacity")]
		[Description("Opacity of colored border. Value between 0 and 255.")]
		[DefaultValue(OpacityDefault)]
		public byte Opacity { get; set; } = OpacityDefault;

		[Category(OpacityAndThicknessCategory)]
		[DisplayName("Thickness")]
		[Description("Thickness of colored border in pixels. Value between 0 and 255.")]
		[DefaultValue(ThicknessDefault)]
		public byte Thickness { get; set; } = ThicknessDefault;

		[Category(BehaviorCategory)]
		[DisplayName("Factor in solution path")]
		[Description("If you copy a solution, it will have different colors. Or if you move a solution it will change its color. If set to false, then colors are independent of absolute path of solution.")]
		[DefaultValue(FactorInSolutionPathDefault)]
		public bool FactorInSolutionPath { get; set; } = FactorInSolutionPathDefault;

		[Category(CustomPaletteCategory)]
		[DisplayName(ReplaceDefaultPaletteDisplayName)]
		[Description("Replace built-in color set instead of appending to it. If custom palette is empty, this option will be ignored.")]
		[DefaultValue(ReplaceDefaultPaletteDefault)]
		public bool ReplaceDefaultPalette { get; set; } = ReplaceDefaultPaletteDefault;

#pragma warning disable CA1819 // Properties should not return arrays
		[Category(CustomPaletteCategory)]
		[DisplayName("Additional colors")]
		[Description("See description of '" + ReplaceDefaultPaletteDisplayName + "' option for more information.")]
		public Color[] CustomColors { get; set; } = Array.Empty<Color>();

		[Category(CustomMappingsCategory)]
		[DisplayName("Custom mappings")]
		[Description("If project name matches wildcard, then corresponding border color will be applied. Wildcards support * and ? operators.")]
		public CustomMapping[] CustomMappings { get; set; } = new[] { new CustomMapping { Wildcard = "*read?only*", Color = Color.Purple } };
#pragma warning restore CA1819 // Properties should not return arrays

		protected override void OnApply(PageApplyEventArgs e)
		{
			base.OnApply(e);
			Global.Services?.SettingsChangedEventDispatcher?.RaiseSettingsChanged(this);
		}

		/// <summary>
		/// VS serialization system refuses to handle arrays correctly so they must be handled manually.
		/// Handling other properties is left to it through <c>base</c> call.
		/// (See https://stackoverflow.com/q/32751040/776442e .)
		/// </summary>
		// NOTE: MiniJson is used to serialize custom mappings because it can be just added as file.
		// Handling packages in VS2015-compatible VSIX is a PITA. When 2015 dropped, things can be simplified and properified.
		public override void SaveSettingsToStorage()
		{
			base.SaveSettingsToStorage();

			var userSettingsStore = GetUserSettingsStore();
			if (!userSettingsStore.CollectionExists(_fullCollectionName))
				userSettingsStore.CreateCollection(_fullCollectionName);

			// Save custom colors.
			using (var stream = new MemoryStream(4))
			{
				foreach (var color in CustomColors)
				{
					stream.WriteByte(color.A);
					stream.WriteByte(color.R);
					stream.WriteByte(color.G);
					stream.WriteByte(color.B);
				}

				stream.Seek(0, SeekOrigin.Begin);

				userSettingsStore.SetMemoryStream(_fullCollectionName, CustomColorsPropertyName, stream);
			}

			// Save custom mappings.
			var dict = CustomMappings.ToDictionary(cm => cm.Wildcard, cm => cm.Color.ToArgb());
			userSettingsStore.SetString(_fullCollectionName, CustomMappingsPropertyName, Json.Serialize(dict));
		}

		/// <summary>
		/// <see cref="SaveSettingsToStorage"/>
		/// </summary>
		public override void LoadSettingsFromStorage()
		{
			base.LoadSettingsFromStorage();

			var userSettingsStore = GetUserSettingsStore();
			if (!userSettingsStore.CollectionExists(_fullCollectionName))
				return;

			// Read custom colors.
			using (var stream = userSettingsStore.GetMemoryStream(_fullCollectionName, CustomColorsPropertyName))
			{
				stream.Seek(0, SeekOrigin.Begin);
				var length = (int)(stream.Length / 4);
				var colors = new List<Color>(length);

				for (var i = 0; i < length; i++)
				{
					colors.Add(Color.FromArgb(stream.ReadByte(), stream.ReadByte(), stream.ReadByte(), stream.ReadByte()));
				}

				CustomColors = colors.ToArray();
			}

			// Read custom mappings.
			if (!userSettingsStore.PropertyExists(_fullCollectionName, CustomMappingsPropertyName))
				return;

			var json = userSettingsStore.GetString(_fullCollectionName, CustomMappingsPropertyName);
			var deserialized = Json.Deserialize(json) as Dictionary<string, object>;
#pragma warning disable IDE0012 // Simplify Names
			CustomMappings = (from kv in deserialized
							  let color = (kv.Value is long) ? (int)(long)kv.Value : 0
							  select new CustomMapping { Wildcard = kv.Key, Color = Color.FromArgb(color) })
							  .ToArray();
#pragma warning restore IDE0012 // Simplify Names
		}

		private static WritableSettingsStore GetUserSettingsStore()
		{
			var settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
			var userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
			return userSettingsStore;
		}
	}
}
