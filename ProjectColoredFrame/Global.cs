namespace ProjectColoredFrame
{
    using System;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// Inevitably every project has at least a few full-blown globals.
    /// Instead of keeping them hidden or disguised as innocent,
    /// why not expose and list them in one place here.
    /// </summary>
    /// <remarks>
    /// Full-blown global is something less innocent as Color.Green or string.Empty,
    /// something closer to MyFavoriteSingletonService.Instance.
    /// </remarks>
    internal static class Global
    {
        public const string Name = "ProjectColoredFrame";
        public const string OptionsPageName = "General";

        //public static event EventHandler<EventArgs> SettingsChanged = delegate { };

        //public static void RaiseSettingsChanged(object sender)
        //{
        //    SettingsChanged(sender, new EventArgs());
        //}

        public static ProjectColoredFramePackage Package
        {
            get
            {
                return ProjectColoredFramePackage.Current;
            }
        }

        public static Properties GetProperties()
        {
            var dte = (DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));
            return dte.Properties[Name, OptionsPageName];
        }
    }
}
