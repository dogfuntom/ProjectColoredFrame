using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectColoredFrame
{
    internal sealed class SettingsChangedEventDispatcher
    {
        public event EventHandler<EventArgs> SettingsChanged = delegate
        { };

        public void RaiseSettingsChanged(object sender)
        {
            SettingsChanged(sender, EventArgs.Empty);
        }
    }
}
