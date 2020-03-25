using System;
using Moq;
using NUnit.Framework;
using SM.Expressions;

namespace VirtualChannels.Tests
{
    [TestFixture]
    public class SingleLoggedParameterVirtualChannelTests
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
        public void AdditionWithSlowRowVirtualChannelTest()
        {
            var expression = SMExpressionParser.ParseOrThrow("$a + nolog($b)");

            var symbolTable = m_repository.Create<IParametersSymbolTable>();
            var slowRowStorage = m_repository.Create<ISlowRowStorage<long>>();


            symbolTable.Setup(x => x.GetId("a", String.Empty, It.IsAny<Action<int>>(), It.IsAny<Action>()))
                .Callback<string, string, Action<int>, Action>((s, s1, ok, ko) => ok(22));

            symbolTable.Setup(x => x.GetId("b", String.Empty, It.IsAny<Action<int>>(), It.IsAny<Action>()))
                .Callback<string, string, Action<int>, Action>((s, s1, ok, ko) => ok(23));

            var vc = new SingleLoggedParameterVirtualChannel<long>(expression, 14, 10 * 1000, symbolTable.Object, slowRowStorage.Object, new MillisTimeUtils());


            slowRowStorage.Setup(x => x.GetValue(1001, 23, false)).Returns(0.1);
            slowRowStorage.Setup(x => x.GetValue(1101, 23, false)).Returns(0.1);
            slowRowStorage.Setup(x => x.GetValue(1201, 23, false)).Returns(0.1);

            var expected = new[] {1.1, 2.1, 3.1};
            double[] output = null;
            vc.AddValues(1001, 22, new double[] {1, 2, 3}, (time, id, values) =>
            {
                Assert.AreEqual(1001, time);
                Assert.AreEqual(14, id);

                output = values;
            });

            CollectionAssert.AreEqual(expected, output);
        }
    }
}
