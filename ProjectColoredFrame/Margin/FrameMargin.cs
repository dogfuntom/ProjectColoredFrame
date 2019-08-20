// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using ProjectColoredFrame.Margin;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectColoredFrame
{
    /// <summary>
    /// Margin's canvas and visual definition including both size and content
    /// </summary>
    internal partial class FrameMargin : Canvas, IWpfTextViewMargin
    {
        private readonly bool _isVertical;
        private readonly Task _initialization;

        private FrameMarginSubscriptions _subscriptions;

        private volatile bool _isUpdating;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameMargin"/> class for a given <paramref name="textView"/>.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> to attach the margin to.</param>
        public FrameMargin(IWpfTextView textView, bool vertical = false)
        {
            // Should be a text document but double-check, just in case.
            if (!textView.TextDataModel.DocumentBuffer.Properties.TryGetProperty(typeof(ITextDocument), out ITextDocument document))
                return;

            _isVertical = vertical;

            _initialization = InitializeAsync(document?.FilePath, textView);

        }

        private async Task InitializeAsync(string filePath, IWpfTextView textView)
        {
            void updateHorizontalHeight(object sender, EventArgs e) { Height = textView.ViewportHeight * (textView.ZoomLevel / 100); }

            var props = new FrameMarginProperties(filePath);
            await UpdateEverythingAsync(props);

            ClipToBounds = true;

            _subscriptions = new FrameMarginSubscriptions(
                (await Global.GetPackageAsync()).Services.MappingEvents,
                textView,
                CreateHandlerForUpdateEverything(props),
                updateHorizontalHeight);

            _subscriptions.Subscribe(_isVertical);
        }

        private EventHandler CreateHandlerForUpdateEverything(FrameMarginProperties props)
        {
            // Have to use async void because it's an event handler.
            async void handleUpdateEverything()
            {
                // Just in case.
                await _initialization;

                await UpdateEverythingAsync(props);
                // If something goes wrong here, the only risk is it can become stuck with True forever. Not critical.
                _isUpdating = false;
            }

            void wrappedHandler(object sender, EventArgs e)
            {
                if (_isDisposed)
                    return;

                if (_isUpdating)
                    return;

                lock (_syncRoot)
                {
                    if (_isUpdating)
                        return;

                    _isUpdating = true;
                }

                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
                handleUpdateEverything();
            }
            return wrappedHandler;
        }

        //// Have to use async void because it's an event handler.
        //private async void HandleUpdateEverything(FrameMarginProperties props)
        //{
        //    // Just in case.
        //    await _initialization;

        //    await UpdateEverythingAsync(props);
        //    // If something goes wrong here, the only risk is it can become stuck with True forever. Not critical.
        //    _isUpdating = false;
        //}

        private async Task UpdateEverythingAsync(FrameMarginProperties props)
        {
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
            }
            else
            {
                Height = thickness;
            }

            Background = borderBrush;
        }
    }
}
