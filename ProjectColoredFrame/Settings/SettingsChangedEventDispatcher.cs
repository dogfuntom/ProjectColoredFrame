// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
