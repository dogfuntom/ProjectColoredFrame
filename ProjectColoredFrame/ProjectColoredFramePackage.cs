using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace ProjectColoredFrame
{
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
    [ProvideProfileAttribute(typeof(ProjectColoredFrameOptionsGrid),
    Global.Name, Global.OptionsPageName, 106, 107, isToolsOptionPage: true, DescriptionResourceID = 108)]
    public sealed class ProjectColoredFramePackage : Package
    {
        private static ProjectColoredFramePackage current;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectColoredFramePackage"/> class.
        /// </summary>
        public ProjectColoredFramePackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
            current = this;
        }

        internal static ProjectColoredFramePackage Current
        {
            get
            {
                // Can't be too cautious.
                current = current.GetService(typeof(Package)) as ProjectColoredFramePackage ?? current;
                return current;
            }
        }

        internal ProjectColoredFrameOptionsGrid OptionsGrid
        {
            get
            {
                return (ProjectColoredFrameOptionsGrid)GetDialogPage(typeof(ProjectColoredFrameOptionsGrid));
            }
        }

        internal SettingsChangedEventDispatcher SettingsChangedEventDispatcher
        {
            get
            {
                return (SettingsChangedEventDispatcher)this.GetService(typeof(SettingsChangedEventDispatcher));
            }
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            var grid = (ProjectColoredFrameOptionsGrid)GetDialogPage(typeof(ProjectColoredFrameOptionsGrid));
            Debug.Write(grid.Opacity);
            base.Initialize();

            var serviceContainer = (IServiceContainer)this.GetService(typeof(IServiceContainer));
            serviceContainer.AddService(typeof(SettingsChangedEventDispatcher), new SettingsChangedEventDispatcher());

            current = this.GetService(typeof(Package)) as ProjectColoredFramePackage;

            // Ensure LoadSettingsFromStorage is called (it won't be called automatically due to bug in VS).
            //Current.OptionsGrid.LoadSettingsFromStorage();
        }
    }
}
