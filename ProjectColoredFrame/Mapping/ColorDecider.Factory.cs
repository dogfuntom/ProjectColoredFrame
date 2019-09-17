// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using EnvDTE;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

namespace ProjectColoredFrame.Mapping
{
    // Not sure if having a factory is not over-architecturing
    // but the initialization logic is too big to keep it together with main responsibility,
    // and it's clearly not a fit for the instantiating class eigther.
    internal partial class ColorDecider
    {
        private static IReadOnlyList<Color> PredefinedPalette => Settings.OptionsStatic.PredefinedPalette;

        // This method is very heavy with nested methods but they're not needed anywhere else,
        // so no benefit in cluttering up the codebase by having them outside of it.
        public static ColorDecider Create(Solution solution, PcfSettings options)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            IReadOnlyList<Color> loadPalette()
            {
                var keep = options.IncludeDefaultPalette;
                var customPalette = options.CustomColors;

                var result = new List<Color>(PredefinedPalette.Count);
                if (keep || customPalette.IsNullOrEmpty())
                {
                    result.AddRange(PredefinedPalette);
                }

                // convert System.Drawing.Color to System.Windows.Media.Color
                var toAdd = from c in customPalette
                            select DrawingColorToMediaColor(c);
                result.AddRange(toAdd);

                return new ReadOnlyCollection<Color>(result);
            }

            var signatureGenerator = new SignatureGenerator(options.FactorInSolutionPath);

            IReadOnlyDictionary<int, int> map(int paletteCount)
            {
                var signatures = (from project in solution.Projects.Cast<Project>()
                                  select signatureGenerator.GetSignature(project.UniqueName, solution.FullName))
                                  .ToList();

                return PaletteDistribution.Map(signatures, paletteCount);
            }

            IReadOnlyList<Color> palette = loadPalette();
            var instance = new ColorDecider(
                solution.FullName,
                signatureGenerator,
                palette,
                map(palette.Count),
                new ReadOnlyCollection<ICustomMapping>(options.CustomMappings.Cast<ICustomMapping>().ToList()));

            instance.DebugLogMapping(solution);
            return instance;
        }

        private static Color DrawingColorToMediaColor(System.Drawing.Color source) => new Color { A = source.A, B = source.B, G = source.G, R = source.R };

        [Conditional("DEBUG")]
        private void DebugLogMapping(Solution solution)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Debug.Indent();
            foreach (Project project in solution)
            {
                Debug.Write(project.Name);
                Debug.Write(" got color ");
                Debug.Write(GetColorOf(project).ToString());
                Debug.WriteLine(" assigned to it.");
            }
            Debug.Unindent();
        }
    }
}
