using System;

namespace ProjectColoredFrame
{
    /// <see cref="MappingEventsDispatcher"/>
    internal interface IMappingEvents
    {
        event EventHandler MappingChanged;
        event EventHandler OptionsChanged;
    }
}