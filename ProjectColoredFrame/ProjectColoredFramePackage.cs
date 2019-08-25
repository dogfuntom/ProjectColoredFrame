﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ProjectColoredFrame.Core;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace ProjectColoredFrame
{

    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(ProjectColoredFramePackageGuids.PackageGuidString)]
    [ProvideOptionPage(typeof(ProjectColoredFrameOptionsGrid), Global.Name, Global.OptionsPageName, 106, 107, true)]
    [ProvideProfile(typeof(ProjectColoredFrameOptionsGrid),
        Global.Name, Global.OptionsPageName, 106, 107, isToolsOptionPage: true, DescriptionResourceID = 108)]
    [ProvideAutoLoad(UIContextGuids80.SolutionHasMultipleProjects, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class ProjectColoredFramePackage : AsyncPackage //-V3072
    {
        private static readonly TaskCompletionSource<ProjectColoredFramePackage> s_current = new TaskCompletionSource<ProjectColoredFramePackage>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectColoredFramePackage"/> class.
        /// </summary>
        public ProjectColoredFramePackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        internal static ProjectColoredFramePackage CurrentUncertain { get; private set; }

        internal static Task<ProjectColoredFramePackage> GetCurrentAsync() => s_current.Task;

        internal ProjectColoredFrameOptionsGrid OptionsGrid => (ProjectColoredFrameOptionsGrid)GetDialogPage(typeof(ProjectColoredFrameOptionsGrid));

        internal Services Services { get; private set; }

        internal async Task<bool> TryWriteToActivityLogAsync(__ACTIVITYLOG_ENTRYTYPE kind, string text)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(DisposalToken);

            // "You should obtain the activity log just before writing to it. Do not cache or save the activity log for future use."
            // See: https://docs.microsoft.com/en-us/visualstudio/extensibility/how-to-use-the-activity-log?view=vs-2017
            var log = await GetServiceAsync(typeof(SVsActivityLog)) as IVsActivityLog;
            if (log == null)
                return false;

            log.LogEntry((uint)kind, ToString(), text);
            return true;
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            // When initialized asynchronously, we *may* be on a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            // Otherwise, remove the switch to the UI thread if you don't need it.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    s_current.SetCanceled();
                    return;
                }

                Services = new Services((DTE2)await GetServiceAsync(typeof(DTE)), this);

                s_current.SetResult(this);
                CurrentUncertain = this;
            }
            catch (ObjectDisposedException)
            {
                await TryWriteToActivityLogAsync(__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR,
                    "ProjectColoredFrame attempted to initialize but it already either was initialized or failed to initialize.");
            }
            catch (InvalidOperationException)
            {
                await TryWriteToActivityLogAsync(__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR,
                    "ProjectColoredFrame attempted to initialize but it already either was initialized or failed to initialize.");
            }
        }
    }
}
