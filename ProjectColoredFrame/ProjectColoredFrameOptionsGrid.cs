using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;

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

		private const string CustomColorsPropertyName = nameof(CustomColors);

		private const byte OpacityDefault = byte.MaxValue / 4;
		private const byte ThicknessDefault = 2;

		private const bool FactorInSolutionPathDefault = false;

		private const bool ReplaceDefaultPaletteDefault = false;

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
		[DisplayName("Replace default palette")]
		[Description("Replace default color set instead of appending to it. If custom palette is empty, this option will be ignored.")]
		[DefaultValue(ReplaceDefaultPaletteDefault)]
		public bool ReplaceDefaultPalette { get; set; } = ReplaceDefaultPaletteDefault;

		[Category(CustomPaletteCategory)]
		[DisplayName("Additional colors")]
		[Description("Additional custom colors.")]
		public Color[] CustomColors { get; set; } = new Color[0];

		protected override void OnApply(PageApplyEventArgs e)
		{
			base.OnApply(e);
			Global.Services?.SettingsChangedEventDispatcher?.RaiseSettingsChanged(this);
		}

		/// <summary>
		/// VS serialization system refuses to handle arrays correctly so <see cref="CustomColors"/> must be handled manually.
		/// Handling other properties is left to it through <c>base</c> call.
		/// (See https://stackoverflow.com/q/32751040/776442e .)
		/// </summary>
		public override void SaveSettingsToStorage()
		{
			base.SaveSettingsToStorage();

			var userSettingsStore = GetUserSettingsStore();
			if (!userSettingsStore.CollectionExists(_fullCollectionName))
				userSettingsStore.CreateCollection(_fullCollectionName);

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
		}

		private static WritableSettingsStore GetUserSettingsStore()
		{
			var settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
			var userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
			return userSettingsStore;
		}
	}
}
