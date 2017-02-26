namespace ProjectColoredFrame
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using ProjectColoredFrame.Core;

    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(ProjectColoredFramePackageGuids.PackageGuidString)]
    [ProvideOptionPage(typeof(ProjectColoredFrameOptionsGrid), Global.Name, Global.OptionsPageName, 106, 107, true)]
    [ProvideProfile(typeof(ProjectColoredFrameOptionsGrid),
        Global.Name, Global.OptionsPageName, 106, 107, isToolsOptionPage: true, DescriptionResourceID = 108)]
    [ProvideAutoLoad(UIContextGuids80.SolutionHasMultipleProjects)]
    public sealed class ProjectColoredFramePackage : Package
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectColoredFramePackage"/> class.
        /// </summary>
        public ProjectColoredFramePackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
            Current = this;
        }

        internal static ProjectColoredFramePackage Current { get; private set; }

        internal bool IsInitialized { get; private set; }

        internal ProjectColoredFrameOptionsGrid OptionsGrid => (ProjectColoredFrameOptionsGrid)GetDialogPage(typeof(ProjectColoredFrameOptionsGrid));

        internal Services Services { get; private set; }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            //var grid = (ProjectColoredFrameOptionsGrid)GetDialogPage(typeof(ProjectColoredFrameOptionsGrid));
            base.Initialize();

            Current = this;

            this.Services = new Services((DTE2)this.GetService(typeof(DTE)));
            this.IsInitialized = true;
        }
    }
}
