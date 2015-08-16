﻿namespace ProjectColoredFrame
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Media;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;
    using static System.Diagnostics.Debug;

    internal static class ColorChooser
    {
        private static Color[] predefined = new Color[]
        {
            (Color)ColorConverter.ConvertFromString("#FFE51400"), // red
            (Color)ColorConverter.ConvertFromString("#FF60A917"), // green
            (Color)ColorConverter.ConvertFromString("#FFE3C800"), // yellow
            (Color)ColorConverter.ConvertFromString("#FF0050EF"), // cobalt
            (Color)ColorConverter.ConvertFromString("#FFD80073"), // magenta
            (Color)ColorConverter.ConvertFromString("#FF00ABA9"), // teal
        };

        private static Dictionary<string, int> mapping = new Dictionary<string, int>();

        //private static bool initialized = false;
        private static Solution initializedForSolution = null;

        public static Color GetColorFor(string fileName)
        {
            Assert(!string.IsNullOrWhiteSpace(fileName));

            var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
            var solution = dte?.Solution;
            var prjItem = solution?.FindProjectItem(fileName);
            var project = prjItem?.ContainingProject;

            var notFound = project == null;
            if (notFound)
                return Colors.Transparent;

            var needReinitialization = solution != initializedForSolution;
            if (needReinitialization)
            {
                mapping.Clear();
                foreach (Project pr in solution.Projects)
                {
                    GetColorFor(pr);
                }
                initializedForSolution = solution;
            }

            return GetColorFor(project);
        }

        private static Color GetColorFor(Project project)
        {
            var uniqueName = project.UniqueName;
            var hash = uniqueName.GetHashCode();

            int index;
            if (mapping.TryGetValue(uniqueName, out index))
                return predefined[index];

            var random = new Random(hash);

            for (int guard = 0; guard < 64; guard++)
            {
                index = random.Next(predefined.Length);
                if (!mapping.Values.Contains(index))
                    break;
            }
            mapping.Add(uniqueName, index);
            return predefined[index];
        }
    }
}
