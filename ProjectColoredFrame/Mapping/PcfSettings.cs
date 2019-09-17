// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using ProjectColoredFrame.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectColoredFrame.Mapping
{
    /// <summary>
    /// There are two classed that contain settings now but there no point for the rest of the code to know about it.
    /// Thus, this facade class is needed.
    /// </summary>
    // Can be struct because it is and should be fully readonly.
    internal struct PcfSettings
    {
        private readonly CustomPaletteOptionPage _customPalette;
        private readonly ProjectColoredFrameOptionGrid _optionGrid;

        public PcfSettings(ProjectColoredFrameOptionGrid optionGrid, CustomPaletteOptionPage customPalette)
        {
            _optionGrid = optionGrid;
            _customPalette = customPalette;
        }

        public bool IncludeDefaultPalette => _customPalette.IncludeBuiltInPalette;

        public IReadOnlyCollection<Color> CustomColors => _customPalette.CustomPalette;

        public bool FactorInSolutionPath => _optionGrid.FactorInSolutionPath;

        public IReadOnlyCollection<CustomMapping> CustomMappings => _optionGrid.CustomMappings;

        public byte Opacity => _optionGrid.Opacity;

        public byte Thickness => _optionGrid.Thickness;
    }
}
