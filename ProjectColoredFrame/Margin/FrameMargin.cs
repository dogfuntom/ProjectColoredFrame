using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace ProjectColoredFrame
{
    /// <summary>
    /// Margin's canvas and visual definition including both size and content
    /// </summary>
    internal class FrameMargin : Canvas, IWpfTextViewMargin
    {
        /// <summary>
        /// Margin name.
        /// </summary>
        public const string MarginName = "FrameMargin";

        /// <summary>
        /// A value indicating whether the object is disposed.
        /// </summary>
        private bool isDisposed;

        private bool isVertical;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameMargin"/> class for a given <paramref name="textView"/>.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> to attach the margin to.</param>
        public FrameMargin(IWpfTextView textView, bool vertical = false)
        {
            byte opacity, thickness;
            GetOptionValues(out opacity, out thickness);

            ITextDocument document;
            if (!textView.TextDataModel.DocumentBuffer.Properties.TryGetProperty(typeof(ITextDocument), out document))
                return;

            var color = !string.IsNullOrWhiteSpace(document?.FilePath)
                ? ColorChooser.GetColorFor(document.FilePath)
                : Colors.Transparent;
            var borderColor = color;
            borderColor.A = opacity;
            var borderBrush = new SolidColorBrush(borderColor);
            borderBrush.Freeze();

            this.MinHeight = this.MinWidth = thickness;

            if (vertical)
            {
                this.Width = thickness;
                EventHandler handler = (sender, e) => { this.Height = textView.ViewportHeight * (textView.ZoomLevel / 100); };
                textView.ViewportHeightChanged += handler;
                textView.ZoomLevelChanged += (sender, e) => handler(sender, e);
            }
            else
                this.Height = thickness;

            this.ClipToBounds = true;
            this.Background = borderBrush;

            this.isVertical = vertical;

            Global.Package.SettingsChangedEventDispatcher.SettingsChanged += HandleSettingsChanged;
        }

        private static void GetOptionValues(out byte opacity, out byte thickness)
        {
            var props = Global.GetProperties();

            opacity = (byte)props.Item("Opacity").Value;
            thickness = (byte)props.Item("Thickness").Value;
        }

        private void HandleSettingsChanged(object sender, EventArgs e)
        {
            byte opacity, thickness;
            GetOptionValues(out opacity, out thickness);

            var col = ((SolidColorBrush)this.Background).Color;
            col.A = opacity;
            this.Background = new SolidColorBrush(col);

            if (this.isVertical)
                this.Width = thickness;
            else
                this.Height = thickness;
        }

        #region IWpfTextViewMargin

        /// <summary>
        /// Gets the <see cref="Sytem.Windows.FrameworkElement"/> that implements the visual representation of the margin.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The margin is disposed.</exception>
        public FrameworkElement VisualElement
        {
            // Since this margin implements Canvas, this is the object which renders
            // the margin.
            get
            {
                this.ThrowIfDisposed();
                return this;
            }
        }


        #endregion

        #region ITextViewMargin

        /// <summary>
        /// Gets a value indicating whether the margin is enabled.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The margin is disposed.</exception>
        public bool Enabled
        {
            get
            {
                this.ThrowIfDisposed();

                // The margin should always be enabled
                return true;
            }
        }

        /// <summary>
        /// Gets the size of the margin.
        /// </summary>
        /// <remarks>
        /// For a horizontal margin this is the height of the margin,
        /// since the width will be determined by the <see cref="ITextView"/>.
        /// For a vertical margin this is the width of the margin,
        /// since the height will be determined by the <see cref="ITextView"/>.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">The margin is disposed.</exception>
        public double MarginSize
        {
            get
            {
                this.ThrowIfDisposed();

                // Since this is a horizontal margin, its width will be bound to the width of the text view.
                // Therefore, its size is its height.
                return this.ActualHeight;
            }
        }
        /// <summary>
        /// Disposes an instance of <see cref="FrameMargin"/> class.
        /// </summary>
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                GC.SuppressFinalize(this);
                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Gets the <see cref="ITextViewMargin"/> with the given <paramref name="marginName"/> or null if no match is found
        /// </summary>
        /// <param name="marginName">The name of the <see cref="ITextViewMargin"/></param>
        /// <returns>The <see cref="ITextViewMargin"/> named <paramref name="marginName"/>, or null if no match is found.</returns>
        /// <remarks>
        /// A margin returns itself if it is passed its own name. If the name does not match and it is a container margin, it
        /// forwards the call to its children. Margin name comparisons are case-insensitive.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="marginName"/> is null.</exception>
        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return string.Equals(marginName, MarginName, StringComparison.OrdinalIgnoreCase) ? this : null;
        }
        #endregion

        /// <summary>
        /// Checks and throws <see cref="ObjectDisposedException"/> if the object is disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(MarginName);
            }
        }
    }
}
