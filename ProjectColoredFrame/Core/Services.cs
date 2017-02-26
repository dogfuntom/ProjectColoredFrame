namespace ProjectColoredFrame.Core
{
    using EnvDTE80;

    internal sealed class Services
    {
        public readonly SettingsChangedEventDispatcher SettingsChangedEventDispatcher;

        private readonly SolutionChangeListener SolutionChangeListener;
        private readonly DTE2 dte;

        public Services(DTE2 dte)
        {
            this.SettingsChangedEventDispatcher = new SettingsChangedEventDispatcher();

            this.SolutionChangeListener = new SolutionChangeListener(dte);
            this.SolutionChangeListener.MappingBecameDirty += (s, e) => this.Remap();

            this.dte = dte;
            this.Remap();
        }

        // TODO: Implement event on mapping change.
        public ColorMapper Mapping { get; private set; }

        private void Remap()
        {
            this.Mapping = new ColorMapper(this.dte.Solution);
        }
    }
}
