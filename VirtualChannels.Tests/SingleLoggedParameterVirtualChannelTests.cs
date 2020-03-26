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
            const int virtualParameterId = 14;
            const int loggedParameterId = 22;
            const int slowRowParameterId = 2003;

            var expression = SMExpressionParser.ParseOrThrow("$a + nolog($b)");

            var symbolTable = m_repository.Create<IParametersSymbolTable>();
            var slowRowStorage = m_repository.Create<ISlowRowStorage<long>>();


            symbolTable.Setup(x => x.GetId("a", String.Empty, It.IsAny<Action<int>>(), It.IsAny<Action>()))
                .Callback<string, string, Action<int>, Action>((s, s1, ok, ko) => ok(loggedParameterId));

            symbolTable.Setup(x => x.GetId("b", String.Empty, It.IsAny<Action<int>>(), It.IsAny<Action>()))
                .Callback<string, string, Action<int>, Action>((s, s1, ok, ko) => ok(slowRowParameterId));

            var vc = new SingleLoggedParameterVirtualChannel<long>(expression, virtualParameterId, loggedParameterId, 10 * 1000, symbolTable.Object, slowRowStorage.Object, new MillisTimeUtils());


            slowRowStorage.Setup(x => x.GetValue(1001, slowRowParameterId, false)).Returns(0.1);
            slowRowStorage.Setup(x => x.GetValue(1101, slowRowParameterId, false)).Returns(0.1);
            slowRowStorage.Setup(x => x.GetValue(1201, slowRowParameterId, false)).Returns(0.1);

            var expected = new[] {1.1, 2.1, 3.1};
            double[] output = null;
            vc.AddValues(1001, loggedParameterId, new double[] {1, 2, 3}, (time, id, values) =>
            {
                Assert.AreEqual(1001, time);
                Assert.AreEqual(virtualParameterId, id);

                output = values;
            });

            CollectionAssert.AreEqual(expected, output);
        }

        [Test]
        public void ComputationVirtualChannelTest()
        {
            const int virtualParameterId = 14;
            const int loggedParameterId = 22;

            var expression = SMExpressionParser.ParseOrThrow("sqrt($a + $a)");

            var symbolTable = m_repository.Create<IParametersSymbolTable>();
            var slowRowStorage = m_repository.Create<ISlowRowStorage<long>>();


            symbolTable.Setup(x => x.GetId("a", String.Empty, It.IsAny<Action<int>>(), It.IsAny<Action>()))
                .Callback<string, string, Action<int>, Action>((s, s1, ok, ko) => ok(loggedParameterId));

            
            var vc = new SingleLoggedParameterVirtualChannel<long>(expression, virtualParameterId, loggedParameterId, 10 * 1000, symbolTable.Object, slowRowStorage.Object, new MillisTimeUtils());

            var expected = new[] {4, 3, 1.414213562373095048};
            double[] output = null;
            vc.AddValues(1001, loggedParameterId, new[] {8, 4.5, 1}, (time, id, values) =>
            {
                Assert.AreEqual(1001, time);
                Assert.AreEqual(virtualParameterId, id);

                output = values;
            });

            CollectionAssert.AreEqual(expected, output);
        }
    }
}
