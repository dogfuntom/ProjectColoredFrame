namespace ProjectColoredFrame
{
    using System;

    internal sealed class SettingsChangedEventDispatcher
    {
        public event EventHandler<EventArgs> SettingsChanged;

        public void RaiseSettingsChanged(object sender)
        {
            this.SettingsChanged?.Invoke(sender, EventArgs.Empty);
        }
    }
}
