// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;

namespace ProjectColoredFrame
{
    internal sealed class MappingEventsDispatcher : IMappingEvents
    {
        public event EventHandler MappingChanged;
        public event EventHandler OptionsChanged;

        public void RaiseMappingChanged(object sender) => MappingChanged?.Invoke(sender, EventArgs.Empty);

        public void RaiseOptionsChanged(object sender) => OptionsChanged?.Invoke(sender, EventArgs.Empty);
    }
}
