using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectColoredFrame
{
    /// <summary>
    /// Page (via WPF User Control) shown in Options that allows editing custom colors.
    /// </summary>
    // See also: https://blog.danskingdom.com/adding-a-wpf-settings-page-to-the-tools-options-dialog-window-for-your-visual-studio-extension/
    public partial class CustomPaletteEditor : UserControl
    {
        internal CustomPaletteEditor(CustomPaletteContext context)
        {
            InitializeComponent();
            Initialize(context);
            LostKeyboardFocus += UserControl_LostKeyboardFocus;
        }

        private void Initialize(CustomPaletteContext context)
        {
            DataContext = context;
            CommandBindings.AddRange(context.Commands);
        }

        // See https://blog.danskingdom.com/adding-a-wpf-settings-page-to-the-tools-options-dialog-window-for-your-visual-studio-extension/
        // TODO: Use this snippet if needed or remove it.
        private void UserControl_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // There are no text boxes in this UC. And check boxes probably don't have this problem.

            //var textBoxes = (sender as UserControl)?.FindVisualChildren<TextBox>();
            //if (textBoxes == null)
            //    return;

            //// Find all TextBoxes in this control force the Text bindings to fire to make sure all changes have been saved.
            //// This is required because if the user changes some text, then clicks on the Options Window's OK button, it closes
            //// the window before the TextBox's Text bindings fire, so the new value will not be saved.
            //foreach (var textBox in textBoxes)
            //{
            //    var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
            //    if (bindingExpression != null)
            //        bindingExpression.UpdateSource();
            //}
        }
    }
}
