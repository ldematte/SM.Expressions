using System;
using NUnit.Framework;

namespace SM.Expressions.Tests
{
    [TestFixture]
    public class FunctionContextTests
    {
        [Test]
        [TestCase("log" , 0D)]
        [TestCase("ln"  , 0D)]
        [TestCase("abs" , 1D)]
        [TestCase("sin" , 0.8414709848078965d)]
        [TestCase("asin", 1.5707963267948966d)]
        [TestCase("sinh", 1.1752011936438014d)]
        [TestCase("cos" , 0.54030230586813977d)]
        [TestCase("acos", 0D)]
        [TestCase("cosh", 1.5430806348152437d)]
        [TestCase("tan" , 1.5574077246549023d)]
        [TestCase("atan", 0.78539816339744828d)]
        [TestCase("tanh", 0.76159415595576485d)]
        [TestCase("sqrt", 1D)]
        [TestCase("exp" , Math.E)]
        public void TestFunctions(string function, double expected)
        {
            var expression = SMExpressionParser.ParseOrThrow($"{function}(1)");

            var simpleExpressionContext = new TestExpressionContext();
            var ev = new EvaluatorVisitor(simpleExpressionContext, new TestSymbolTable(), new DefaultFunctionContext());
            expression.Accept(ev);

            Assert.AreEqual(expected, ev.Value, 1e-9);
        }
    }
}
