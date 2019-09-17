using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace ProjectColoredFrame
{
    /// <summary>
    /// ViewModel-like mediator between the editor and its owner.
    /// The goal is to gather all bindings together in one place,
    /// and not to follow the proper pattern, hence such name.
    /// </summary>
    internal class CustomPaletteContext : DependencyObject, ICustomPaletteEditorContext, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IncludeBuiltInPaletteProperty = DependencyProperty.Register(
                nameof(IncludeBuiltInPalette),
                typeof(bool),
                typeof(CustomPaletteContext),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(HandlePropertyChanged)));

        public static readonly RoutedCommand AddColorCmd = new RoutedCommand();
        public static readonly RoutedCommand RemoveColorCmd = new RoutedCommand();

        public readonly ICollection Commands;

        public event PropertyChangedEventHandler PropertyChanged;

        public CustomPaletteContext()
        {
            FullColors = new ObservableCollection<Color>(BuiltInColors.OrderBy(GetHue));
            CustomColors.CollectionChanged += CustomColors_CollectionChanged;

            Commands = new CommandBindingCollection(new [] {
                new CommandBinding(AddColorCmd, AddColorCmdExecuted),
                new CommandBinding(RemoveColorCmd, RemoveColorCmdExecuted)
            });
        }

        public IReadOnlyList<Color> BuiltInColors { get; } = Settings.OptionsStatic.PredefinedPalette;

        public ObservableCollection<Color> CustomColors { get; } = new ObservableCollection<Color>();

        public ObservableCollection<Color> FullColors { get; }

        public bool IncludeBuiltInPalette
        {
            get { return (bool)GetValue(IncludeBuiltInPaletteProperty); }
            set { SetValue(IncludeBuiltInPaletteProperty, value); }
        }

        public bool HasCustomPalette => CustomColors.Count > 0;

        public void AddColor(ColorDialog colorDialog)
        {
            if (colorDialog.ShowDialog() != DialogResult.OK)
                return;

            CustomColors.Add(new Color
            {
                R = colorDialog.Color.R,
                G = colorDialog.Color.G,
                B = colorDialog.Color.B,
                ScA = 1
            });
        }

        public void RemoveColor(Color color) => CustomColors.Remove(color);

        private static float GetHue(Color color) => System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B).GetHue();

        private static void HandlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as CustomPaletteContext)?.UpdateFullColors();

        private void AddColorCmdExecuted(object sender, ExecutedRoutedEventArgs e) => AddColor(new ColorDialog());

        private void RemoveColorCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var color = e.Parameter as Color?;
            if (color.HasValue)
                RemoveColor(color.Value);
        }

        private void CustomColors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldStartingIndex > 0 && e.NewStartingIndex > 0)
                return;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasCustomPalette)));
            UpdateFullColors();
        }

        private void UpdateFullColors()
        {
            if (!HasCustomPalette)
                IncludeBuiltInPalette = true;

            var all = (IncludeBuiltInPalette ? BuiltInColors : Enumerable.Empty<Color>())
                            .Concat(CustomColors)
                            .OrderBy(GetHue);

            FullColors.Clear();
            foreach (var color in all)
            {
                FullColors.Add(color);
            }
        }
    }
}
