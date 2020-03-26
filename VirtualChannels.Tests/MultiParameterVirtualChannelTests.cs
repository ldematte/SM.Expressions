using System;
using Moq;
using NUnit.Framework;
using SM.Expressions;

namespace VirtualChannels.Tests
{
    [TestFixture]
    public class MultiParameterVirtualChannelTests
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
            const int loggedParameterA = 22;
            const int loggedParameterC = 23;
            const int slowRowParameterId = 2003;

            var expression = SMExpressionParser.ParseOrThrow("$a + nolog($b) + $c");

            var symbolTable = m_repository.Create<IParametersSymbolTable>();
            var slowRowStorage = m_repository.Create<ISlowRowStorage<long>>();


            symbolTable.Setup(x => x.GetId("a", string.Empty, It.IsAny<Action<int>>(), It.IsAny<Action>()))
                .Callback<string, string, Action<int>, Action>((s, s1, ok, ko) => ok(loggedParameterA));

            symbolTable.Setup(x => x.GetId("c", string.Empty, It.IsAny<Action<int>>(), It.IsAny<Action>()))
                .Callback<string, string, Action<int>, Action>((s, s1, ok, ko) => ok(loggedParameterC));

            symbolTable.Setup(x => x.GetId("b", string.Empty, It.IsAny<Action<int>>(), It.IsAny<Action>()))
                .Callback<string, string, Action<int>, Action>((s, s1, ok, ko) => ok(slowRowParameterId));

            var vc = new MultiParameterVirtualChannel<long>(expression, virtualParameterId, 
                new []{ loggedParameterA, loggedParameterC }, 10 * 1000, symbolTable.Object, slowRowStorage.Object, new MillisTimeUtils());


            slowRowStorage.Setup(x => x.GetValue(1001, slowRowParameterId, false)).Returns(0.1);
            slowRowStorage.Setup(x => x.GetValue(1101, slowRowParameterId, false)).Returns(0.1);
            slowRowStorage.Setup(x => x.GetValue(1201, slowRowParameterId, false)).Returns(0.1);

            var expected = new[] {12.1, 14.1, 16.1};
            double[] output = null;
            
            vc.AddValues(1001, loggedParameterA, new double[] {1, 2, 3}, (time, id, values) =>
            {
                output = values;
            });

            Assert.IsNull(output);
            
            vc.AddValues(1001, loggedParameterC, new double[] {11, 12, 13}, (time, id, values) =>
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
            const int loggedParameterA = 22;
            const int loggedParameterC = 23;

            var expression = SMExpressionParser.ParseOrThrow("sqrt($a + $c)");

            var symbolTable = m_repository.Create<IParametersSymbolTable>();
            var slowRowStorage = m_repository.Create<ISlowRowStorage<long>>();


            symbolTable.Setup(x => x.GetId("a", String.Empty, It.IsAny<Action<int>>(), It.IsAny<Action>()))
                .Callback<string, string, Action<int>, Action>((s, s1, ok, ko) => ok(loggedParameterA));

            symbolTable.Setup(x => x.GetId("c", String.Empty, It.IsAny<Action<int>>(), It.IsAny<Action>()))
                .Callback<string, string, Action<int>, Action>((s, s1, ok, ko) => ok(loggedParameterC));

            
            var vc = new MultiParameterVirtualChannel<long>(expression, virtualParameterId, 
                new []{loggedParameterA, loggedParameterC}, 10 * 1000, symbolTable.Object, slowRowStorage.Object, new MillisTimeUtils());

            var expected = new[] {4, 3, 1.414213562373095048};
            double[] output = null;
            vc.AddValues(1001, loggedParameterA, new[] {8, 4.5, 1, 3}, (time, id, values) =>
            {
                output = values;
            });

            Assert.IsNull(output);
            vc.AddValues(930, loggedParameterC, new[] {7, 8, 4.5, 1}, (time, id, values) =>
            {
                Assert.AreEqual(1030, time);
                Assert.AreEqual(virtualParameterId, id);

                output = values;
            });

            CollectionAssert.AreEqual(expected, output);
        }
    }
}