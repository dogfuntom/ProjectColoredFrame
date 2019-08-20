// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using Microsoft.VisualStudio.Text.Editor;
using System;

namespace ProjectColoredFrame.Margin
{
    internal sealed class FrameMarginSubscriptions
    {
        private readonly IMappingEvents _mappingEvents;
        private readonly IWpfTextView _textView;
        private readonly EventHandler _updateFrame;
        private readonly EventHandler _updateVerticalHeight;
        private readonly EventHandler<ZoomLevelChangedEventArgs> _handleZoomLevelChanged;

        public FrameMarginSubscriptions(IMappingEvents mappingEvents, IWpfTextView textView, EventHandler updateFrame, EventHandler updateVerticalHeight)
        {
            _mappingEvents = mappingEvents;
            _textView = textView;
            _updateFrame = updateFrame;
            _updateVerticalHeight = updateVerticalHeight;
            _handleZoomLevelChanged = (sender, e) => _updateVerticalHeight?.Invoke(sender, e);
        }

        public void Subscribe(bool isVertical)
        {
            // External changes.
            {
                _mappingEvents.OptionsChanged += _updateFrame;
                _mappingEvents.MappingChanged += _updateFrame;
            }

            // Own changes (changes of tab itself).
            if (isVertical)
            {
                _textView.ViewportHeightChanged += _updateVerticalHeight;
                _textView.ZoomLevelChanged += _handleZoomLevelChanged;
            }
        }

        public void Unsubscribe()
        {
            // External changes.
            _mappingEvents.OptionsChanged -= _updateFrame;
            _mappingEvents.MappingChanged -= _updateFrame;

            // Own changes (changes of tab itself).
            // Don't care if horizontal and vertical, unsubscribing is always safe.
            _textView.ViewportHeightChanged -= _updateVerticalHeight;
            _textView.ZoomLevelChanged -= _handleZoomLevelChanged;
        }
    }
}
