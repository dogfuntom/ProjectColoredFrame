namespace ProjectColoredFrame
{
    using EnvDTE;
    using EnvDTE80;
    using ProjectColoredFrame.Core;

    /// <summary>
    /// Inevitably every project has at least a few all-out globals.
    /// Instead of keeping them hidden or disguised as something minor,
    /// why not expose and list them in one place here.
    /// </summary>
    internal static class Global
    {
#pragma warning disable CC0021 // Use nameof. It's used in persistance, better not change it ever, but namespace can be renamed freely.
		public const string Name = "ProjectColoredFrame";
#pragma warning restore CC0021 // Use nameof
		public const string OptionsPageName = "General";

        private static DTE2 DTE => (DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));

        public static bool IsPackageInitialized => Package?.IsInitialized == true;

        public static ProjectColoredFramePackage Package => ProjectColoredFramePackage.Current;

        public static Properties Properties => DTE.Properties[Name, OptionsPageName];

        public static Services Services => Package.Services;
    }
}
