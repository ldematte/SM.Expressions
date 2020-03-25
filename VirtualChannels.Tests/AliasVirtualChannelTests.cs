using NUnit.Framework;

namespace VirtualChannels.Tests
{
    [TestFixture]
    public class AliasVirtualChannelTests
    {
        [Test]
        public void OutputEqualsInputTest()
        {
            var vc = new AliasVirtualChannel<int>(12, 14);

            var input = new double[] {1, 2, 3};

            double[] output = null;
            vc.AddValues(4, 12, input, (time, id, values) =>
            {
                output = values;
                Assert.AreEqual(4, time);
                Assert.AreEqual(14, id);
            });

            CollectionAssert.AreEqual(input, output);
        }
    }
}
