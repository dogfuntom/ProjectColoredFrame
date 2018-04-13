using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using EnvDTE;

namespace ProjectColoredFrame.Mapping
{
	// Not sure if having a factory is not over-architecturing
	// but the initialization logic is too big to keep it together with main responsibility,
	// and it's clearly not a fit for the instantiating class eigther.
	partial class ColorDecider
	{
		private static readonly IReadOnlyList<Color> s_predefinedPalette = new ReadOnlyCollection<Color>(new[] {
			(Color)ColorConverter.ConvertFromString("#FFE51400"), // red
            (Color)ColorConverter.ConvertFromString("#FF60A917"), // green
            (Color)ColorConverter.ConvertFromString("#FFE3C800"), // yellow
            (Color)ColorConverter.ConvertFromString("#FF0050EF"), // cobalt
            (Color)ColorConverter.ConvertFromString("#FFD80073"), // magenta
            (Color)ColorConverter.ConvertFromString("#FF00ABA9")  // teal
        });

		// This method is very heavy with nested methods but they're not needed anywhere else,
		// so no benefit in cluttering up the codebase by having them outside of it.
		public static ColorDecider Create(Solution solution, ProjectColoredFrameOptionsGrid options)
		{
			IReadOnlyList<Color> loadPalette()
			{
				var replace = options.ReplaceDefaultPalette;
				System.Drawing.Color[] customPalette = options.CustomColors;

				var result = new List<Color>(s_predefinedPalette.Count);
				if ((!replace) || customPalette.IsNullOrEmpty())
					result.AddRange(s_predefinedPalette);

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
