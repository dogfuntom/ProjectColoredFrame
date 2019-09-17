using Microsoft.VisualStudio.Text.Editor;
using Moq;
using ProjectColoredFrame.Margin;
using System;
using Xunit;

namespace ProjectColoredFrame.UnitTest
{
    /// <summary>
    /// At least a cursory attempt at getting memory leak detection into a unit test.  By 
    /// no means a thorough example because I can't accurately simulate Visual Studio 
    /// integration without starting Visual Studio.  But this should at least help me catch
    /// a portion of them. 
    /// </summary>
    public sealed class MemoryLeakTest
    {
        readonly Mock<IMappingEvents> _mappingEvents = new Mock<IMappingEvents>();
        Action _onMappingChange;
        Action _onViewChange;
        readonly FrameMarginSubscriptions _subs;
        readonly Mock<IWpfTextView> _textView = new Mock<IWpfTextView>();

        private void RunGarbageCollector()
        {
            for (var i = 0; i < 15; i++)
            {
                GC.Collect(2, GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();
                GC.Collect(2, GCCollectionMode.Forced);
                GC.Collect();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void FmsIsNotLeakedDueToEvents(bool isVertical)
        {
            var subs = new FrameMarginSubscriptions(
                _mappingEvents.Object,
                _textView.Object,
                (_, __) => _onMappingChange?.Invoke(),
                (_, __) => _onViewChange?.Invoke());

            var weakSubs = new WeakReference(subs);

            subs.Subscribe(isVertical);
            subs.Unsubscribe();
            subs = null;

            RunGarbageCollector();

            Assert.Null(weakSubs.Target);
        }
    }
}
