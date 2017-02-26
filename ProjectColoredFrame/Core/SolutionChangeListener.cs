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
