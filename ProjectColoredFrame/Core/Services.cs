using EnvDTE80;
using ProjectColoredFrame.Mapping;

namespace ProjectColoredFrame.Core
{
	/// <summary>
	/// Lightweight alternative to VS-services. Stores instances of needed system services
	/// and service-like class instances of this codebase itself.
	/// Somewhat similar to Service Locator.
	/// </summary>
	internal sealed class Services
	{
		public readonly SettingsChangedEventDispatcher SettingsChangedEventDispatcher;
		private readonly DTE2 _dte;

		private readonly SolutionChangeListener _solutionChangeListener;

		public Services(DTE2 dte)
		{
			SettingsChangedEventDispatcher = new SettingsChangedEventDispatcher();

			_solutionChangeListener = new SolutionChangeListener(dte);
			_solutionChangeListener.MappingBecameDirty += (s, e) => Remap();

			_dte = dte;
			Remap();
		}

		public ColorDecider Mapping { get; private set; }

		private void Remap()
		{
			//Mapping = new ColorDecider(_dte.Solution, new SignatureGenerator(ProjectColoredFramePackage.Current.OptionsGrid.FactorInSolutionPath));
			Mapping = ColorDecider.Create(_dte.Solution, ProjectColoredFramePackage.Current.OptionsGrid);
		}
	}
}
