namespace ProjectColoredFrame
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Media;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;

    internal sealed class ColorMapper
    {
        private static readonly IReadOnlyList<Color> PredefinedPalette = new ReadOnlyCollection<Color>(new[] {
            (Color)ColorConverter.ConvertFromString("#FFE51400"), // red
            (Color)ColorConverter.ConvertFromString("#FF60A917"), // green
            (Color)ColorConverter.ConvertFromString("#FFE3C800"), // yellow
            (Color)ColorConverter.ConvertFromString("#FF0050EF"), // cobalt
            (Color)ColorConverter.ConvertFromString("#FFD80073"), // magenta
            (Color)ColorConverter.ConvertFromString("#FF00ABA9")  // teal
        });

        /// <summary>
        /// The mapping from project unique name hash to index of color in palette.
        /// </summary>
        private readonly IReadOnlyDictionary<int, int> mapping;

        private readonly IReadOnlyList<Color> palette = LoadPalette();

        private readonly Solution solution;

        public ColorMapper(Solution solution)
        {
            this.mapping = Map(solution, this.palette);
            this.solution = solution;

            this.DebugLogMapping();
        }

        public Color GetColorOf(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return Colors.Transparent;

            var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
            var solution = dte?.Solution;
            var prjItem = solution?.FindProjectItem(fileName);
            var project = prjItem?.ContainingProject;

            if (project == null)
                return Colors.Transparent;
            else
                return this.GetColorOf(project);
        }

        private static IReadOnlyDictionary<int, int> Map(Solution solution, IReadOnlyList<Color> palette)
        {
            Projects projects = solution.Projects;

            var postponed = new List<Tuple<Project, Random>>(projects.Count);
            var mapping = new Dictionary<int, int>();

            /* Randomly get color using project name as seed, for consistency.
            This approach is adopted in hope of more stable coloring on solution contents changes.
            Avoid conflicts while possible by posponing them.*/

            foreach (Project project in projects)
            {
                var nameHash = project.UniqueName.GetHashCode();

                var random = new Random(nameHash);
                var index = random.Next(palette.Count);

                if (mapping.ContainsValue(index))
                    postponed.Add(new Tuple<Project, Random>(project, random));
                else
                    mapping.Add(nameHash, index);
            }

            // Resolve postponed conflicting mappings.
            foreach (var postponedItem in postponed)
            {
                var nameHash = postponedItem.Item1.UniqueName.GetHashCode();
                var random = postponedItem.Item2;

                // Check for hash conflict (unlikely, but far from impossible).
                if (mapping.ContainsKey(nameHash))
                    continue;

                // Try a few times to find unused color, otherwise give up and use any.
                var index = -1;
                for (int i = 0; i < 64; i++)
                {
                    index = random.Next(palette.Count);
                    if (!mapping.ContainsValue(index))
                        break;
                }

                // Use index we have settled on.
                mapping.Add(nameHash, index);
            }

            return new ReadOnlyDictionary<int, int>(mapping);
        }

        private static IReadOnlyList<Color> LoadPalette()
        {
            var options = ProjectColoredFramePackage.Current.OptionsGrid;
            var replace = options.ReplaceDefaultPalette;
            System.Drawing.Color[] customPalette = options.CustomColors;

            var result = new List<Color>(PredefinedPalette.Count);
            if ((!replace) || customPalette.IsNullOrEmpty())
                result.AddRange(PredefinedPalette);

            // convert System.Drawing.Color to System.Windows.Media.Color
            var toAdd = from c in customPalette
                        select DrawingColorToMediaColor(c);
            result.AddRange(toAdd);

            return new ReadOnlyCollection<Color>(result);
        }

        private static Color DrawingColorToMediaColor(System.Drawing.Color source)
        {
            return new Color
            {
                A = source.A,
                B = source.B,
                G = source.G,
                R = source.R
            };
        }

        [Conditional("DEBUG")]
        private void DebugLogMapping()
        {
            Debug.Indent();
            foreach (Project project in this.solution)
            {
                Debug.Write(project.Name);
                Debug.Write(" got color ");
                Debug.Write(this.GetColorOf(project).ToString());
                Debug.WriteLine(" assigned to it.");
            }
            Debug.Unindent();
        }

        /// <summary>
        /// Gets the color of.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        private Color GetColorOf(Project project)
        {
            if (project == null)
                return Colors.Transparent;

            var index = -1;
            this.mapping.TryGetValue(project.UniqueName.GetHashCode(), out index);

            return index >= 0 ? this.palette[index] : Colors.Transparent;
        }
    }
}
