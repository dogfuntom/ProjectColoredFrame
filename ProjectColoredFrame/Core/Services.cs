// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using EnvDTE80;
using ProjectColoredFrame.Mapping;

namespace ProjectColoredFrame.Core
{
	/// <summary>
	/// Lightweight imitation of VS's services. Stores instances of needed system services
	/// and service-like class instances of this codebase itself.
	/// Somewhat similar to Service Locator.
	/// </summary>
	internal sealed class Services
	{
		public readonly MappingEventsDispatcher MappingEvents;

		private readonly DTE2 _dte;
		private readonly SolutionChangeListener _solutionChangeListener;

		public Services(DTE2 dte, ProjectColoredFramePackage package)
		{
			MappingEvents = new MappingEventsDispatcher();

			_solutionChangeListener = new SolutionChangeListener(dte);
			_solutionChangeListener.MappingBecameDirty += (s, e) => Remap(package);

            MappingEvents.OptionsChanged += (s, e) => RemapSilently(package);

			_dte = dte;
			Remap(package);
		}

		public ColorDecider Mapping { get; private set; }

        // TODO: It's weird that Services handles remap on relevant events, clearly breaks SRP.
		private void Remap(ProjectColoredFramePackage package)
        {
            RemapSilently(package);
            MappingEvents.RaiseMappingChanged(Mapping);
        }

        private void RemapSilently(ProjectColoredFramePackage package) => Mapping = ColorDecider.Create(_dte.Solution, package.Settings);
    }
}
