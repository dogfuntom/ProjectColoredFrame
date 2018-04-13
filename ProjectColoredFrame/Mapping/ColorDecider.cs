using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace ProjectColoredFrame.Mapping
{
	internal sealed partial class ColorDecider
	{
		private readonly IReadOnlyList<ICustomMapping> _customMappings;

		/// <summary>
		/// The mapping from project signature to index of color in palette.
		/// </summary>
		private readonly IReadOnlyDictionary<int, int> _mapping;

		private readonly IReadOnlyList<Color> _palette;
		private readonly SignatureGenerator _signatureGenerator;

		private readonly string _solutionFullName;

		private ColorDecider(
			string solutionFullName,
			SignatureGenerator signatureGenerator,
			IReadOnlyList<Color> palette,
			IReadOnlyDictionary<int, int> mapping,
			IReadOnlyList<ICustomMapping> customMappings)
		{
			_solutionFullName = solutionFullName;
			_signatureGenerator = signatureGenerator;
			_palette = palette;
			_mapping = mapping;
			_customMappings = customMappings;
		}

		public Color GetColorOf(string fileName)
		{
			if (string.IsNullOrWhiteSpace(fileName))
				return Colors.Transparent;

			var project = (Package.GetGlobalService(typeof(DTE)) as DTE2)?.Solution?.FindProjectItem(fileName)?.ContainingProject;

			return project == null ? Colors.Transparent : GetColorOf(project);
		}

		private static bool IsMatch(string name, string wildcard)
		{
			// From: https://stackoverflow.com/a/30300521/776442
			string wildCardToRegular(string value) => "^" + Regex.Escape(value).Replace(@"\?", ".").Replace(@"\*", ".*") + "$";

			return Regex.IsMatch(name, wildCardToRegular(wildcard), RegexOptions.Compiled);
		}

		private Color GetColorOf(Project project)
		{
			if (project == null)
				return Colors.Transparent;

			foreach (var custom in _customMappings)
			{
				if (IsMatch(project.Name, custom.Wildcard))
					return DrawingColorToMediaColor(custom.Color);
			}

			return _mapping.TryGetValue(_signatureGenerator.GetSignature(project.UniqueName, _solutionFullName), out var index) ? _palette[index] : Colors.Transparent;
		}
	}
}
