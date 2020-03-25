using System;
using NUnit.Framework;

namespace VirtualChannels.Tests
{
    [TestFixture]
    public class TimeUtilsTests
    {
        [Test]
        public void MillisIndexToTimeTest()
        {
            var t = new MillisTimeUtils();

            Assert.AreEqual(402, t.IndexToTime(2, 4, 10000));
            Assert.AreEqual(0, t.IndexToTime(0, 0, 10000));

            var oneDay = TimeSpan.FromDays(1).TotalMilliseconds;

            Assert.AreEqual(oneDay + 400, t.IndexToTime((long)oneDay, 4, 10000));
        }

        [Test]
        public void BigNumbersMillisTimeTest()
        {
            var t = new MillisTimeUtils();
            
            var oneDay = (long)TimeSpan.FromDays(1).TotalMilliseconds;

            var indexAtOneDay = t.TimeToIndex(oneDay, oneDay, 1000 * 1000);

            var timeFromIndex = t.IndexToTime(0, indexAtOneDay, 1000 * 1000);

            var time = TimeSpan.FromMilliseconds(timeFromIndex);

            Assert.AreEqual(TimeSpan.Zero, time);

            indexAtOneDay = t.TimeToIndex(0, oneDay, 1000 * 1000);

            timeFromIndex = t.IndexToTime(oneDay, indexAtOneDay, 1000 * 1000);

            time = TimeSpan.FromMilliseconds(timeFromIndex);

            Assert.AreEqual(TimeSpan.FromDays(2), time);
        }


        [Test]
        public void HugeNumbersMillisTimeTest()
        {
            var t = new MillisTimeUtils();

            var frequency = 25 * 1000 * 1000;
            
            var oneWeek = (long)TimeSpan.FromDays(7).TotalMilliseconds;

            var indexAtOneWeek = t.TimeToIndex(0, oneWeek, frequency);

            var timeFromIndex = t.IndexToTime(0, indexAtOneWeek, frequency);

            var time = TimeSpan.FromMilliseconds(timeFromIndex);

            Assert.AreEqual(TimeSpan.FromDays(7), time);
        }



        [Test]
        public void DateTimeIndexToTimeTest()
        {
            var t = new DateTimeTimeUtils();

            var now = DateTime.Now;

            Assert.AreEqual(now.AddMilliseconds(400), t.IndexToTime(now, 4, 10000));
            Assert.AreEqual(now, t.IndexToTime(now, 0, 10000));
            Assert.AreEqual(t.Zero(), t.IndexToTime(t.Zero(), 0, 10000));

            var oneDay = TimeSpan.FromDays(1);

            Assert.AreEqual(now.Add(oneDay), t.IndexToTime(now, (long)oneDay.TotalSeconds, 1000));
            Assert.AreEqual(now.Add(oneDay), t.IndexToTime(now, (long)oneDay.TotalMilliseconds, 1000 * 1000));
        }

        [Test]
        public void BigNumbersDateTimeTimeTest()
        {
            var t = new DateTimeTimeUtils();

            var oneDay = TimeSpan.FromDays(1);
            var zero = t.Zero();

            var indexAtOneDay = t.TimeToIndex(zero, zero.Add(oneDay), 1000 * 1000);

            var timeFromIndex = t.IndexToTime(zero, indexAtOneDay, 1000 * 1000);

            var time = timeFromIndex.Subtract(zero);

            Assert.AreEqual(TimeSpan.FromDays(1), time);
        }


        [Test]
        public void HugeNumbersDateTimeTimeTest()
        {
            var t = new DateTimeTimeUtils();

            var frequency = 25 * 1000 * 1000;
            
            var oneWeek = TimeSpan.FromDays(7);
            var zero = t.Zero();

            var indexAtOneWeek = t.TimeToIndex(zero, zero.Add(oneWeek), frequency);

            var timeFromIndex = t.IndexToTime(zero, indexAtOneWeek, frequency);

            var time = timeFromIndex.Subtract(zero);

            Assert.AreEqual(TimeSpan.FromDays(7), time);
        }
    }
}