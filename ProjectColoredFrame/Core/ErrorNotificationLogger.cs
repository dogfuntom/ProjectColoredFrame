// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace ProjectColoredFrame
{
    internal static class ErrorNotificationLogger
    {
#pragma warning disable VSTHRD100 // Avoid async void methods. Can't help it because this method must be available for code that cannot await.
        public static async void LogErrorWithoutShowingErrorNotificationUI(string message, Exception e)
        {
            // TODO: Try Exceptionless here.
            // (Can't try it together with other features because it's likely to break because adding packages into an extension is finicky.)

            _ = await ProjectColoredFramePackage.CurrentUncertain?.TryWriteToActivityLogAsync(__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR,
                                                                                              message + Environment.NewLine + e);
        }
#pragma warning restore VSTHRD100 // Avoid async void methods
    }
}