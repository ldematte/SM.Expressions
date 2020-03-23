using System;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;


namespace SM.Expressions.Tests
{
    [TestFixture]
    public class ExpressionParserTests
    {
        [Test]
        public void ParseAndDumpSimpleTest()
        {
            var expression = SMExpressionParser.ParseOrThrow("log(10) + $aaa + 1*2");

            var sb = new StringBuilder();
            expression.Accept(new ToStringExpressionVisitor(sb));

            Assert.AreEqual("((log(10)+$aaa:)+(1*2))", sb.ToString());
        }

        [Test]
        public void ParseAndDumpNoLogTest()
        {
            var expression = SMExpressionParser.ParseOrThrow("log(10) + $aaa + abs(nolog($adc_Controller)) + 1*2");

            var sb = new StringBuilder();
            expression.Accept(new ToStringExpressionVisitor(sb));

            Assert.AreEqual("(((log(10)+$aaa:)+abs(nolog($adc:Controller)))+(1*2))", sb.ToString());
        }

        [Test]
        public void ParseAndEvaluatePerformanceTest()
        {
            var expression = SMExpressionParser.ParseOrThrow("log(10) + $aaa + abs(nolog($adc_Controller)) + 1*2");

            var simpleExpressionContext = new TestExpressionContext();

            var times = 1000000;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < times; ++i)
            {
                var evaluator = new EvaluatorVisitor(simpleExpressionContext, new DefaultFunctionContext());
                expression.Accept(evaluator);
            }

            var elapsed = sw.ElapsedMilliseconds;

            Console.WriteLine($"Evaluated: {elapsed}ms");
        }

        [Test]
        public void ParseAndEvaluateTest()
        {
            var expression = SMExpressionParser.ParseOrThrow("log(10) + $aaa + abs(nolog($adc_Controller)) + 1*2");

            var simpleExpressionContext = new TestExpressionContext();
            var ev = new EvaluatorVisitor(simpleExpressionContext, new DefaultFunctionContext());
            expression.Accept(ev);

            Assert.AreEqual(14.0, ev.Value, 1e-9);
        }

        [Test]
        public void ParseAndCompilePerformanceTest()
        {
            var expression = SMExpressionParser.ParseOrThrow("log(10) + $aaa + abs(nolog($adc_Controller)) + 1*2");

            var simpleExpressionContext = new TestExpressionContext();

            var compiler = new CompileVisitor(simpleExpressionContext, new DefaultCallContext());
            expression.Accept(compiler);
            var compiledExpression = compiler.GetCompiledExpression();

            var times = 1000000;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < times; ++i)
            {
                compiledExpression();
            }

            var elapsed = sw.ElapsedMilliseconds;
            Console.WriteLine($"Compiled: {elapsed}ms");
        }

        [Test]
        public void ParseAndCompileTest()
        {
            var expression = SMExpressionParser.ParseOrThrow("log(10) + $aaa + abs(nolog($adc_Controller)) + 1*2");

            var simpleExpressionContext = new TestExpressionContext();

            var compiler = new CompileVisitor(simpleExpressionContext, new DefaultCallContext());
            expression.Accept(compiler);
            var compiledExpression = compiler.GetCompiledExpression();

            Assert.AreEqual(14.0, compiledExpression(), 1e-9);
        }
    }
}

