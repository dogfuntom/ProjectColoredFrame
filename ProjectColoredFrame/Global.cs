namespace ProjectColoredFrame
{
    using System;

    internal static class Global
    {
        public const string Name = "ProjectColoredFrame";
        public const string OptionsPageName = "General";

        public static event EventHandler<EventArgs> SettingsChanged = delegate { };

        public static void RaiseSettingsChanged(object sender)
        {
            SettingsChanged(sender, new EventArgs());
        }
    }
}
