﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace ProjectColoredFrame
{
    using EnvDTE;
    using EnvDTE80;
    using System.Threading.Tasks;

    /// <summary>
    /// Inevitably every project has at least a few full-blown globals.
    /// Instead of hiding them and pretending that they are nothing important,
    /// why not expose them by listing in one place (here).
    /// </summary>
    internal static class Global
    {
        public const string OptionCategoryName = "ProjectColoredFrame";
        public const string GeneralOptionsName = "General";
        public const string CustomPaletteOptionsName = "Custom Colors";

        //private static DTE2 Dte => (DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));

        /// <summary>
        /// Wait for (already happening) initialization and get this extension's package.
        /// </summary>
        public static Task<ProjectColoredFramePackage> GetPackageAsync() => ProjectColoredFramePackage.GetCurrentAsync();
    }
}
