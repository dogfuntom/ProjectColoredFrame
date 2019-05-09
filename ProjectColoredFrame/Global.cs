// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace ProjectColoredFrame
{
    using EnvDTE;
    using EnvDTE80;
    using ProjectColoredFrame.Core;
    using System.Threading.Tasks;

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

        public static Task<ProjectColoredFramePackage> GetPackageAsync() => ProjectColoredFramePackage.GetCurrentAsync();

        public static Properties Properties => DTE.Properties[Name, OptionsPageName];
    }
}
