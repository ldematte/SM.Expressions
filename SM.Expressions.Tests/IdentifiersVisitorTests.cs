using System.Collections.Generic;
using NUnit.Framework;

namespace SM.Expressions.Tests
{
    [TestFixture]
    public class IdentifiersVisitorTests
    {
        [Test]
        public void NoLogIsJustForIdentifiersTest()
        {
            var expression = SMExpressionParser.ParseOrThrow("nolog($a) ^ (nolog(sqrt($b)))"); 

            var identifiers = new Dictionary<string, SMIdentifier>();
            expression.Accept(new IdentifiersVisitor(identifiers));

            Assert.AreEqual(2, identifiers.Count);
            Assert.IsTrue(identifiers["a"].IsNoLog);
            Assert.IsFalse(identifiers["b"].IsNoLog);
        }

        [Test]
        public void RawIdentifiersAndNoLogTest()
        {
            var expression = SMExpressionParser.ParseOrThrow("nolog($$a) ^ (sqrt($$b)) + $c"); 

            var identifiers = new Dictionary<string, SMIdentifier>();
            expression.Accept(new IdentifiersVisitor(identifiers));

            Assert.AreEqual(3, identifiers.Count);
            Assert.IsTrue(identifiers["a"].IsNoLog);
            Assert.IsFalse(identifiers["b"].IsNoLog);
            Assert.IsFalse(identifiers["c"].IsNoLog);
        }

        [Test]
        public void RegularWinsOverNoLogTest()
        {
            var expression = SMExpressionParser.ParseOrThrow("nolog($a) ^ $a"); 

            var identifiers = new Dictionary<string, SMIdentifier>();
            expression.Accept(new IdentifiersVisitor(identifiers));

            Assert.AreEqual(1, identifiers.Count);
            Assert.IsFalse(identifiers["a"].IsNoLog);
        }
    }
}
