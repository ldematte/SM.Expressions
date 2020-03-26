using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using VirtualChannels.DataStructures;

namespace VirtualChannels.Tests.DataStructures
{
    [TestFixture]
    public class CoverageStateMachineTests
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
        public void WithFixedStateFactoryTest()
        {
            var buffers = new CircularArray<double>[3];
            
            buffers[0] = new CircularArray<double>(10, double.NaN);
            buffers[1] = new CircularArray<double>(10, double.NaN);
            buffers[2] = new CircularArray<double>(10, double.NaN);

            buffers[0][2] = 0;
            buffers[0][3] = 0;
            
            buffers[0][5] = 0;

            buffers[2][2] = 2;
            buffers[2][3] = 2;
            
            buffers[2][5] = 2;
            buffers[2][6] = 2;


            buffers[1].Insert(new double[] {1, 1, 1, 1, 1}, 1);


            var sm = new CoverageStateMachine(new FixedCoverageStateFactory());

            var foundSegments = new List<Tuple<long, long>>();

            for (int i = 0; i < 10; ++i)
            {
                sm.Consume(buffers, 1, i, (from, to) => foundSegments.Add(Tuple.Create(from, to)));
            }
            sm.Done((from, to) => foundSegments.Add(Tuple.Create(from, to)));

            var expected = new List<Tuple<long, long>>
            {
                Tuple.Create(2L, 3L),
                Tuple.Create(5L, 5L)
            };

            CollectionAssert.AreEqual(expected, foundSegments);
        }

        [Test]
        public void ToTheEndWithFixedStateFactoryTest()
        {
            var buffers = new CircularArray<double>[3];
            
            buffers[0] = new CircularArray<double>(10, double.NaN);
            buffers[1] = new CircularArray<double>(10, double.NaN);
            buffers[2] = new CircularArray<double>(10, double.NaN);

            buffers[0][7] = 0;
            buffers[0][8] = 0;
            buffers[0][9] = 0;

            buffers[2][2] = 2;
            buffers[2][3] = 2;
            
            buffers[2][8] = 2;
            buffers[2][9] = 2;


            buffers[1].Insert(new double[] {1, 1, 1, 1, 1}, 5);


            var sm = new CoverageStateMachine(new FixedCoverageStateFactory());

            var foundSegments = new List<Tuple<long, long>>();

            for (int i = 0; i < 10; ++i)
            {
                sm.Consume(buffers, 1, i, (from, to) => foundSegments.Add(Tuple.Create(from, to)));
            }
            sm.Done((from, to) => foundSegments.Add(Tuple.Create(from, to)));
            
            var expected = new List<Tuple<long, long>>
            {
                Tuple.Create(8L, 9L)
            };

            CollectionAssert.AreEqual(expected, foundSegments);
        }

        [Test]
        public void ToTheEndStateSuccessionTest()
        {
            var buffers = new CircularArray<double>[3];
            
            buffers[0] = new CircularArray<double>(10, double.NaN);
            buffers[1] = new CircularArray<double>(10, double.NaN);
            buffers[2] = new CircularArray<double>(10, double.NaN);

            buffers[0][7] = 0;
            buffers[0][8] = 0;
            buffers[0][9] = 0;

            buffers[2][2] = 2;
            buffers[2][3] = 2;
            
            buffers[2][8] = 2;
            buffers[2][9] = 2;


            buffers[1].Insert(new double[] {1, 1, 1, 1, 1}, 5);


            var stateFactory = m_repository.Create<ICoverageStateFactory>();

            var searchState = m_repository.Create<ICoverageState>();
            var extendState = m_repository.Create<ICoverageState>();

            stateFactory.Setup(x => x.Searching()).Returns(searchState.Object);
            for (int i = 0; i < 8; ++i)
            {
                var index = i;
                searchState
                    .Setup(x => x.Consume(It.IsAny<CircularArray<double>[]>(), 1, index, It.IsAny<Action<long, long>>()))
                    .Returns(searchState.Object);
            }
            searchState
                .Setup(x => x.Consume(It.IsAny<CircularArray<double>[]>(), It.IsAny<int>(), 8, It.IsAny<Action<long, long>>()))
                .Returns(extendState.Object);

            extendState
                .Setup(x => x.Consume(It.IsAny<CircularArray<double>[]>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<Action<long, long>>()))
                .Returns(extendState.Object);
            extendState
                .Setup(x => x.Done(It.IsAny<Action<long, long>>()))
                .Returns(searchState.Object)
                .Callback<Action<long, long>>(action => action(8, 9));

            var sm = new CoverageStateMachine(stateFactory.Object);

            var foundSegments = new List<Tuple<long, long>>();

            for (int i = 0; i < 10; ++i)
            {
                sm.Consume(buffers, 1, i, (from, to) => foundSegments.Add(Tuple.Create(from, to)));
            }
            sm.Done((from, to) => foundSegments.Add(Tuple.Create(from, to)));
            
            var expected = new List<Tuple<long, long>>
            {
                Tuple.Create(8L, 9L)
            };

            CollectionAssert.AreEqual(expected, foundSegments);
        }
    }
}