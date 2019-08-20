// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using EnvDTE;
using EnvDTE80;
using System;

namespace ProjectColoredFrame.Core
{
    /// <summary>
    /// Tracks changes of current solution or in current solution that are relevant for palette mapping.
    /// </summary>
    internal class SolutionChangeListener
    {
        public event EventHandler MappingBecameDirty;

        public SolutionChangeListener(DTE2 dte)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            var events = dte.Events.SolutionEvents;
            events.Opened += () => OnMappingBecameDirty();
            events.ProjectAdded += OnMappingBecameDirty;
            events.ProjectRenamed += (p, _) => OnMappingBecameDirty(p);
            events.ProjectRemoved += OnMappingBecameDirty;
        }

        private void OnMappingBecameDirty(Project project = null) => MappingBecameDirty?.Invoke(this, EventArgs.Empty);
    }
}
