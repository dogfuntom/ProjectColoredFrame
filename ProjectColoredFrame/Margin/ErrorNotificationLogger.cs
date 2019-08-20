// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;

namespace ProjectColoredFrame
{
    internal class ErrorNotificationLogger
    {
        internal static void LogErrorWithoutShowingErrorNotificationUI(string message, Exception e)
        {
            // TODO: Try Exceptionless here.
            // (Can't try it together with other features because it's likely to break because adding packages into an extension is finicky.)
        }
    }
}