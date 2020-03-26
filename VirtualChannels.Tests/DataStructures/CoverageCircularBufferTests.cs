using Moq;
using NUnit.Framework;
using VirtualChannels.DataStructures;

namespace VirtualChannels.Tests.DataStructures
{
    [TestFixture]
    public class CoverageCircularBufferTests
    {
        private MockRepository m_repository;

        [SetUp]
        public void SetUp()
        {
            m_repository = new MockRepository(MockBehavior.Strict);
        }

        [TearDown]
        public void TearDown()
        {
            m_repository.VerifyAll();
        }

        [Test]
        public void SegmentsReadyCallbackTest()
        {
            var b = new CoverageCircularBuffer<long>(new []{14, 15, 16}, 10 * 1000, 10, new MillisTimeUtils());

            var called = false;
            b.AddSamples(1000, 14, new double[] {14, 14, 14, 14}, (time, count, segments) =>
            {
                called = true;
            });
            Assert.IsFalse(called);

            called = false;
            b.AddSamples(1030, 15, new double[] {15, 15, 15, 15}, (time, count, segments) =>
            {
                called = true;
            });
            Assert.IsFalse(called);

            called = false;
            b.AddSamples(1070, 16, new double[] {16, 16, 16, 16}, (time, count, segments) =>
            {
                called = true;
                Assert.AreEqual(1070, time);
                Assert.AreEqual(3, count);
                
                Assert.AreEqual(3, segments.Count);

                for (int i = 0; i < count; ++i)
                    Assert.AreEqual(14, segments[0][i]);

                for (int i = 0; i < count; ++i)
                    Assert.AreEqual(15, segments[1][i]);

                for (int i = 0; i < count; ++i)
                    Assert.AreEqual(16, segments[2][i]);
            });
            Assert.IsTrue(called);

        }


        [Test]
        public void SegmentsReadyAfterRetroFillCallbackTest()
        {
            var b = new CoverageCircularBuffer<long>(new []{14, 15, 16}, 10 * 1000, 10, new MillisTimeUtils());

            var called = false;
            b.AddSamples(1000, 14, new double[] {14, 14, 14, 14}, (time, count, segments) =>
            {
                called = true;
            });
            Assert.IsFalse(called);

            called = false;
            b.AddSamples(1030, 15, new double[] {15, 15, 15, 15}, (time, count, segments) =>
            {
                called = true;
            });
            Assert.IsFalse(called);

            called = false;
            b.AddSamples(1070, 16, new double[] {16, 16, 16, 16}, (time, count, segments) =>
            {
                called = true;
            });
            Assert.IsTrue(called);


            called = false;
            b.AddSamples(0, 14, new double[] {14, 14, 14, 14, 14, 14}, (time, count, segments) =>
            {
                called = true;
            });
            Assert.IsFalse(called);

            called = false;
            b.AddSamples(0, 15, new double[] {15, 15, 15, 15, 15, 15}, (time, count, segments) =>
            {
                called = true;
            });
            Assert.IsFalse(called);

            var calledTimes = 0;
            b.AddSamples(200, 16, new double[] {16, 16, 16, 16, 16, 16, 16, 16, 16}, (time, count, segments) =>
            {
                ++calledTimes;

                if (calledTimes == 1)
                {
                    Assert.AreEqual(200, time);
                    Assert.AreEqual(4, count);

                    Assert.AreEqual(3, segments.Count);

                    for (int i = 0; i < count; ++i)
                        Assert.AreEqual(14, segments[0][i]);

                    for (int i = 0; i < count; ++i)
                        Assert.AreEqual(15, segments[1][i]);

                    for (int i = 0; i < count; ++i)
                        Assert.AreEqual(16, segments[2][i]);
                }
                else
                {
                    Assert.AreEqual(1000, time);
                    Assert.AreEqual(1, count);

                    Assert.AreEqual(3, segments.Count);

                    for (int i = 0; i < count; ++i)
                        Assert.AreEqual(14, segments[0][i]);

                    for (int i = 0; i < count; ++i)
                        Assert.AreEqual(15, segments[1][i]);

                    for (int i = 0; i < count; ++i)
                        Assert.AreEqual(16, segments[2][i]);
                }
            });
            Assert.AreEqual(2, calledTimes);
        }
    }
}