// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectColoredFrame
{
    /// <summary>
    /// Margin's canvas and visual definition including both size and content
    /// </summary>
    internal class FrameMargin : Canvas, IWpfTextViewMargin
    {
#pragma warning disable CC0021 // Use nameof
        /// <summary>
        /// Margin name.
        /// </summary>
        public const string MarginName = "FrameMargin";
#pragma warning restore CC0021 // Use nameof

        /// <summary>
        /// A value indicating whether the object is disposed.
        /// </summary>
        private bool _isDisposed;

        private readonly bool _isVertical;
        private readonly Task _initialization;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameMargin"/> class for a given <paramref name="textView"/>.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> to attach the margin to.</param>
        public FrameMargin(IWpfTextView textView, bool vertical = false)
        {
            // Check if not a text document, just in case.
            if (!textView.TextDataModel.DocumentBuffer.Properties.TryGetProperty(typeof(ITextDocument), out ITextDocument document))
                return;

            _isVertical = vertical;

            _initialization = InitializeAsync(document?.FilePath, textView);

        }

        private async Task InitializeAsync(string filePath, IWpfTextView textView)
        {
            var props = new FrameMarginProperties(filePath);
            await props.UpdateAsync();

            var borderColor = props.Color;
            borderColor.A = props.Opacity;
            var borderBrush = new SolidColorBrush(borderColor);
            borderBrush.Freeze();

            var thickness = props.Thickness;
            MinHeight = MinWidth = thickness;

            if (_isVertical)
            {
                Width = thickness;
                EventHandler handler = (sender, e) => { Height = textView.ViewportHeight * (textView.ZoomLevel / 100); };
                textView.ViewportHeightChanged += handler;
                textView.ZoomLevelChanged += (sender, e) => handler(sender, e);
            }
            else
            {
                Height = thickness;
            }

            ClipToBounds = true;
            Background = borderBrush;

            // TODO: Also watch for solution change too.
            var services = (await Global.GetPackageAsync()).Services;
            services.SettingsChangedEventDispatcher.SettingsChanged += HandleSettingsChanged;
        }

        private void HandleSettingsChanged(object sender, EventArgs e)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            var opacity = (byte)Global.Properties.Item("Opacity").Value;
            var thickness = (byte)Global.Properties.Item("Thickness").Value;

            var col = ((SolidColorBrush)Background).Color;
            col.A = opacity;
            Background = new SolidColorBrush(col);

            if (_isVertical)
                Width = thickness;
            else
                Height = thickness;
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
                ThrowIfDisposed();
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
                ThrowIfDisposed();

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
                ThrowIfDisposed();

                // Since this is a horizontal margin, its width will be bound to the width of the text view.
                // Therefore, its size is its height.
                return ActualHeight;
            }
        }

        /// <summary>
        /// Disposes an instance of <see cref="FrameMargin"/> class.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                GC.SuppressFinalize(this);
                _isDisposed = true;
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
            if (_isDisposed)
            {
                throw new ObjectDisposedException(MarginName);
            }
        }
    }
}
