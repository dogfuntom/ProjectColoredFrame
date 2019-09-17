using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace ProjectColoredFrame
{
    public interface ICustomPaletteEditorContext
    {
        IReadOnlyList<Color> BuiltInColors { get; }

        ObservableCollection<Color> CustomColors { get; }

        ObservableCollection<Color> FullColors { get; }

        bool HasCustomPalette { get; }

        bool IncludeBuiltInPalette { get; set; }
    }
}