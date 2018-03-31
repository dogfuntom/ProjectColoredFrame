using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Media;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System.Collections.Immutable;

namespace ProjectColoredFrame.Mapping
{
	internal sealed class ColorDecider
	{
		private static readonly IReadOnlyList<Color> s_predefinedPalette = new ReadOnlyCollection<Color>(new[] {
			(Color)ColorConverter.ConvertFromString("#FFE51400"), // red
            (Color)ColorConverter.ConvertFromString("#FF60A917"), // green
            (Color)ColorConverter.ConvertFromString("#FFE3C800"), // yellow
            (Color)ColorConverter.ConvertFromString("#FF0050EF"), // cobalt
            (Color)ColorConverter.ConvertFromString("#FFD80073"), // magenta
            (Color)ColorConverter.ConvertFromString("#FF00ABA9")  // teal
        });

		/// <summary>
		/// The mapping from project signature to index of color in palette.
		/// </summary>
		private readonly IReadOnlyDictionary<int, int> _mapping;

		private readonly IReadOnlyList<Color> _palette = LoadPalette();
		private readonly SignatureGenerator _signatureGenerator;

		private readonly Solution _solution;

		public ColorDecider(Solution solution, SignatureGenerator signatureGenerator)
		{
			_signatureGenerator = signatureGenerator;
			_solution = solution;

			_mapping = Map(_solution, _palette);

			DebugLogMapping();
		}

		public Color GetColorOf(string fileName)
		{
			if (string.IsNullOrWhiteSpace(fileName))
				return Colors.Transparent;

			var project = (Package.GetGlobalService(typeof(DTE)) as DTE2)?.Solution?.FindProjectItem(fileName)?.ContainingProject;

			return project == null ? Colors.Transparent : GetColorOf(project);
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

		private static IReadOnlyList<Color> LoadPalette()
		{
			var options = ProjectColoredFramePackage.Current.OptionsGrid;
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

		[Conditional("DEBUG")]
		private void DebugLogMapping()
		{
			Debug.Indent();
			foreach (Project project in _solution)
			{
				Debug.Write(project.Name);
				Debug.Write(" got color ");
				Debug.Write(GetColorOf(project).ToString());
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

			return _mapping.TryGetValue(_signatureGenerator.GetSignature(project.UniqueName, _solution.FullName), out var index) ? _palette[index] : Colors.Transparent;
		}

		private IReadOnlyDictionary<int, int> Map(Solution solution, IReadOnlyList<Color> palette)
		{
			var signatures = (from project in solution.Projects.Cast<Project>()
							  select _signatureGenerator.GetSignature(project.UniqueName, solution.FullName))
							  .ToImmutableArray();

			return PaletteDistribution.Map(signatures, palette.Count);
		}
	}
}
