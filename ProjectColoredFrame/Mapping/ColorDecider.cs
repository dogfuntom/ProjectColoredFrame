using System.Collections.Generic;
using System.Windows.Media;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace ProjectColoredFrame.Mapping
{
	internal sealed partial class ColorDecider
	{
		/// <summary>
		/// The mapping from project signature to index of color in palette.
		/// </summary>
		private readonly IReadOnlyDictionary<int, int> _mapping;

		private readonly IReadOnlyList<Color> _palette;
		private readonly SignatureGenerator _signatureGenerator;

		private readonly string _solutionFullName;

		private ColorDecider(string solutionFullName, SignatureGenerator signatureGenerator, IReadOnlyList<Color> palette, IReadOnlyDictionary<int, int> mapping)
		{
			_solutionFullName = solutionFullName;
			_signatureGenerator = signatureGenerator;
			_palette = palette;
			_mapping = mapping;
		}

		public Color GetColorOf(string fileName)
		{
			if (string.IsNullOrWhiteSpace(fileName))
				return Colors.Transparent;

			var project = (Package.GetGlobalService(typeof(DTE)) as DTE2)?.Solution?.FindProjectItem(fileName)?.ContainingProject;

			return project == null ? Colors.Transparent : GetColorOf(project);
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

			return _mapping.TryGetValue(_signatureGenerator.GetSignature(project.UniqueName, _solutionFullName), out var index) ? _palette[index] : Colors.Transparent;
		}
	}
}
