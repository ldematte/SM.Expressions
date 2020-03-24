using System;
using NUnit.Framework;
using VirtualChannels.DataStructures;

namespace VirtualChannels.Tests.DataStructures
{
    [TestFixture]
    public class CircularArrayTests
    {
        [Test]
        public void EmptyArrayAlwaysReturnsNoElementTest()
        {
            var a = new CircularArray<double>(5, double.NaN);

            Assert.AreEqual(double.NaN, a[0]);
            Assert.AreEqual(double.NaN, a[4]);
            Assert.AreEqual(double.NaN, a[500]);
        }

        [Test]
        public void WriteInRangeAndReadBackTest()
        {
            var a = new CircularArray<double>(5, double.NaN);

            a[0] = 1;
            Assert.AreEqual(1, a[0]);

            a[2] = 2;
            Assert.AreEqual(2, a[2]);
            Assert.AreEqual(double.NaN, a[1]);
        }

        [Test]
        public void WriteBeyondCurrentEndMovesWindowTest()
        {
            var a = new CircularArray<double>(5, double.NaN);

            Assert.AreEqual(double.NaN, a[0]);
            Assert.AreEqual(double.NaN, a[6]);

            a[0] = 1;
            Assert.AreEqual(1, a[0]);

            a[2] = 2;
            Assert.AreEqual(2, a[2]);
            Assert.AreEqual(double.NaN, a[1]);

            a[6] = 6;
            Assert.AreEqual(double.NaN, a[0]);
            Assert.AreEqual(2, a[2]);
            Assert.AreEqual(6, a[6]);
        }

        [Test]
        public void WriteBeforeCurrentWindowHasNoEffectTest()
        {
            var a = new CircularArray<double>(5, double.NaN);

            a[0] = 1;
            Assert.AreEqual(1, a[0]);

            a[6] = 6;
            Assert.AreEqual(double.NaN, a[0]);
            Assert.AreEqual(6, a[6]);

            a[0] = 0;
            a[1] = 1;
            a[2] = 2;
            a[3] = 3;
            a[4] = 4;
            a[5] = 5;

            Assert.AreEqual(double.NaN, a[0]);
            Assert.AreEqual(double.NaN, a[1]);
            Assert.AreEqual(2, a[2]);
            Assert.AreEqual(3, a[3]);
            Assert.AreEqual(4, a[4]);
            Assert.AreEqual(5, a[5]);
        }

        [Test]
        public void MovingForwardWithGapLeavesCleanGap()
        {
            var a = new CircularArray<double>(5, double.NaN);

            a[6] = 6;

            a[0] = 0;
            a[1] = 1;
            a[2] = 2;
            a[3] = 3;
            a[4] = 4;
            a[5] = 5;

            Assert.AreEqual(double.NaN, a[0]);
            Assert.AreEqual(double.NaN, a[1]);
            Assert.AreEqual(2, a[2]);
            Assert.AreEqual(3, a[3]);
            Assert.AreEqual(4, a[4]);
            Assert.AreEqual(5, a[5]);

            a[9] = 9;

            Assert.AreEqual(double.NaN, a[3]);
            Assert.AreEqual(double.NaN, a[4]);
            Assert.AreEqual(5, a[5]);
            Assert.AreEqual(6, a[6]);
            Assert.AreEqual(double.NaN, a[7]);
            Assert.AreEqual(double.NaN, a[8]);
        }

        [Test]
        public void MovingForwardWithBigGapErasesEverything()
        {
            var a = new CircularArray<double>(5, double.NaN);

            a[6] = 6;

            a[0] = 0;
            a[1] = 1;
            a[2] = 2;
            a[3] = 3;
            a[4] = 4;
            a[5] = 5;

            Assert.AreEqual(double.NaN, a[0]);
            Assert.AreEqual(double.NaN, a[1]);
            Assert.AreEqual(2, a[2]);
            Assert.AreEqual(3, a[3]);
            Assert.AreEqual(4, a[4]);
            Assert.AreEqual(5, a[5]);
            Assert.AreEqual(6, a[6]);

            a[11] = 11;

            Assert.AreEqual(double.NaN, a[0]);
            Assert.AreEqual(double.NaN, a[1]);
            Assert.AreEqual(double.NaN, a[2]);
            Assert.AreEqual(double.NaN, a[3]);
            Assert.AreEqual(double.NaN, a[4]);
            Assert.AreEqual(double.NaN, a[5]);
            Assert.AreEqual(double.NaN, a[6]);
            Assert.AreEqual(double.NaN, a[7]);
            Assert.AreEqual(double.NaN, a[8]);
            Assert.AreEqual(double.NaN, a[9]);
            Assert.AreEqual(double.NaN, a[10]);
            Assert.AreEqual(11, a[11]);

            Assert.AreEqual(double.NaN, a[12]);
        }

        [Test]
        public void ReadBeyondWindowDoesNotTouchContent()
        {
            var a = new CircularArray<double>(5, double.NaN);

            a[6] = 6;

            a[0] = 0;
            a[1] = 1;
            a[2] = 2;
            a[3] = 3;
            a[4] = 4;
            a[5] = 5;

            Assert.AreEqual(double.NaN, a[0]);
            Assert.AreEqual(double.NaN, a[1]);
            Assert.AreEqual(2, a[2]);
            Assert.AreEqual(3, a[3]);
            Assert.AreEqual(4, a[4]);
            Assert.AreEqual(5, a[5]);
            Assert.AreEqual(6, a[6]);

            Assert.AreEqual(double.NaN, a[12]);

            Assert.AreEqual(double.NaN, a[0]);
            Assert.AreEqual(double.NaN, a[1]);
            Assert.AreEqual(2, a[2]);
            Assert.AreEqual(3, a[3]);
            Assert.AreEqual(4, a[4]);
            Assert.AreEqual(5, a[5]);
            Assert.AreEqual(6, a[6]);
        }

        [Test]
        public void MovingWindowALotForwardTest()
        {
            var a = new CircularArray<double>(5, double.NaN);

            a[6] = 6;

            a[0] = 0;
            a[1] = 1;
            a[2] = 2;
            a[3] = 3;
            a[4] = 4;
            a[5] = 5;

            Assert.AreEqual(double.NaN, a[0]);
            Assert.AreEqual(double.NaN, a[1]);
            Assert.AreEqual(2, a[2]);
            Assert.AreEqual(3, a[3]);
            Assert.AreEqual(4, a[4]);
            Assert.AreEqual(5, a[5]);
            Assert.AreEqual(6, a[6]);

            a[323] = 323;

            a[327] = 327;
            a[326] = 326;

            for (int i = 0; i < 323; ++i)
            {
                Assert.AreEqual(double.NaN, a[i]);
            }

            Assert.AreEqual(323, a[323]);
            Assert.AreEqual(double.NaN, a[324]);
            Assert.AreEqual(double.NaN, a[325]);
            Assert.AreEqual(326, a[326]);
            Assert.AreEqual(327, a[327]);

            for (int i = 328; i < 400; ++i)
            {
                Assert.AreEqual(double.NaN, a[i]);
            }
        }
    }
}
