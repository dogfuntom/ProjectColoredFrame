// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMColor = System.Windows.Media.Color;
using DColor = System.Drawing.Color;
using System.Windows;
using System.Windows.Media;

namespace ProjectColoredFrame
{
    internal static class Extensions
    {
        public static byte ClampToByte(this int @this) => (byte)Math.Max(byte.MinValue, Math.Min(byte.MaxValue, @this));

        /// <summary>
        /// Recursively finds the visual children of the given control.
        /// <seealso cref="https://blog.danskingdom.com/adding-a-wpf-settings-page-to-the-tools-options-dialog-window-for-your-visual-studio-extension/"/>
        /// </summary>
        /// <typeparam name="T">The type of control to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            if (dependencyObject != null)
            {
                for (int index = 0; index < VisualTreeHelper.GetChildrenCount(dependencyObject); index++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, index);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static bool IsNullOrEmpty<T>(this IReadOnlyCollection<T> @this)
        {
            if (@this == null)
                return true;

            return @this.Count == 0;
        }

        public static DColor ToWindowsForms(this WMColor @this) => DColor.FromArgb(@this.A, @this.R, @this.G, @this.B);

        public static WMColor ToWpf(this DColor @this) => WMColor.FromArgb(@this.A, @this.R, @this.G, @this.B);

    }
}
