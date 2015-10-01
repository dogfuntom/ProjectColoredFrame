namespace ProjectColoredFrame
{
    using System;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;

    internal static class Global
    {
        public const string Name = "ProjectColoredFrame";
        public const string OptionsPageName = "General";

        public static event EventHandler<EventArgs> SettingsChanged = delegate { };

        public static void RaiseSettingsChanged(object sender)
        {
            SettingsChanged(sender, new EventArgs());
        }

        public static Properties GetProperties()
        {
            var dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            return dte.Properties[Name, OptionsPageName];
        }
    }
}
