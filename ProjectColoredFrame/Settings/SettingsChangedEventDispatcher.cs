namespace ProjectColoredFrame
{
    using System;

	// TODO: Consider renaming to Options..
    internal sealed class SettingsChangedEventDispatcher
    {
        public event EventHandler<EventArgs> SettingsChanged;

        public void RaiseSettingsChanged(object sender)
        {
            this.SettingsChanged?.Invoke(sender, EventArgs.Empty);
        }
    }
}
