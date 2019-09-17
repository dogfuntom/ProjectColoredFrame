using Moq;
using Xunit;
using ProjectColoredFrame.Margin;
using Microsoft.VisualStudio.Text.Editor;
using System;

namespace ProjectColoredFrame.UnitTest
{
#pragma warning disable CA1063 // Implement IDisposable Correctly
    public class FrameMarginSubscriptionsTest : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        readonly Mock<IMappingEvents> _mappingEvents = new Mock<IMappingEvents>();
        Action _onMappingChange;
        Action _onViewChange;
        readonly FrameMarginSubscriptions _subs;
        readonly Mock<IWpfTextView> _textView = new Mock<IWpfTextView>();

        public FrameMarginSubscriptionsTest()
        {
            _subs = new FrameMarginSubscriptions(
                _mappingEvents.Object,
                _textView.Object,
                (_, __) => _onMappingChange?.Invoke(),
                (_, __) => _onViewChange?.Invoke());
        }

#pragma warning disable CC0029 // Disposables Should Call Suppress Finalize
#pragma warning disable CA1063 // Implement IDisposable Correctly
        public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
#pragma warning restore CC0029 // Disposables Should Call Suppress Finalize
        {
            _subs.Unsubscribe();
        }

        [Fact]
        public void DoesNotReactOnViewChangeIfHorizontal()
        {
            var actual = 0;

            _onViewChange = () => actual++;
            _subs.Subscribe(isVertical: false);

            Assert.Equal(0, actual);

            _textView.Raise(tv => tv.ViewportHeightChanged += null, EventArgs.Empty);
            Assert.Equal(0, actual);

            var zlcea = new Mock<ZoomLevelChangedEventArgs>(100, null);
            _textView.Raise(tv => tv.ZoomLevelChanged += null, zlcea.Object);
            Assert.Equal(0, actual);
        }

        [Fact]
        public void ReactsOnViewChangeIfVertical()
        {
            var actual = 0;

            _onViewChange = () => actual++;
            _subs.Subscribe(isVertical: true);

            Assert.Equal(0, actual);

            _textView.Raise(tv => tv.ViewportHeightChanged += null, EventArgs.Empty);
            Assert.Equal(1, actual);

            var zlcea = new Mock<ZoomLevelChangedEventArgs>(100, null);
            _textView.Raise(tv => tv.ZoomLevelChanged += null, zlcea.Object);
            Assert.Equal(2, actual);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void UpdatesOnMappingOrOptionChange(bool isVertical)
        {
            var actual = 0;

            _onMappingChange = () => actual++;
            _subs.Subscribe(isVertical);

            Assert.Equal(0, actual);

            _mappingEvents.Raise(me => me.MappingChanged += null, EventArgs.Empty);
            Assert.Equal(1, actual);

            _mappingEvents.Raise(me => me.OptionsChanged += null, EventArgs.Empty);
            Assert.Equal(2, actual);
        }
    }
}
