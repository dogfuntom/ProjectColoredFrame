namespace ProjectColoredFrame
{
    using EnvDTE;
    using EnvDTE80;
    using ProjectColoredFrame.Core;

    /// <summary>
    /// Inevitably every project has at least a few full-blown globals.
    /// Instead of keeping them hidden or disguised as something minor,
    /// why not expose and list them in one place here.
    /// </summary>
    /// <remarks>
    /// Here full-blown global means something less innocent than Color.Green or string.Empty,
    /// i.e. something closer to MyFavoriteSingletonService.Instance.
    /// </remarks>
    internal static class Global
    {
        public const string Name = "ProjectColoredFrame";
        public const string OptionsPageName = "General";

        private static DTE2 DTE => (DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));

        public static bool IsPackageInitialized => Package?.IsInitialized == true;

        public static ProjectColoredFramePackage Package => ProjectColoredFramePackage.Current;

        public static Properties Properties => DTE.Properties[Name, OptionsPageName];

        public static Services Services => Package.Services;
    }
}
