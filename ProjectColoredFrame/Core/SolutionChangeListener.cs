// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace ProjectColoredFrame.Core
{
    using System;
    using EnvDTE;
    using EnvDTE80;

    /// <summary>
    /// Tracks changes of current solution or in current solution that are relevant for palette mapping.
    /// </summary>
    internal class SolutionChangeListener
    {
        public event EventHandler MappingBecameDirty;

        public SolutionChangeListener(DTE2 dte)
        {
            var events = dte.Events.SolutionEvents;
            events.Opened += () => this.OnMappingBecameDirty();
            events.ProjectAdded += this.OnMappingBecameDirty;
            events.ProjectRenamed += (p, _) => this.OnMappingBecameDirty(p);
            events.ProjectRemoved += this.OnMappingBecameDirty;
        }

        private void OnMappingBecameDirty(Project project = null)
        {
            this.MappingBecameDirty?.Invoke(this, EventArgs.Empty);
        }
    }
}
