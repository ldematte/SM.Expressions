using System.Text;
using NUnit.Framework;

namespace SM.Expressions.Tests
{
    [TestFixture]
    public class PrecedenceTests
    {
        [Test]
        public void OperatorsPrecedenceTest()
        {
            var expression = SMExpressionParser.ParseOrThrow("$a + $b - $c * +$d / $e ^ $f << 3 | 4 & 5 >> 6");

            var sb = new StringBuilder();
            expression.Accept(new ToStringExpressionVisitor(sb));

            Assert.AreEqual("(((($a:+$b:)-(($c:*$d:)/($e:^$f:)))<<3)|(4&(5>>6)))", sb.ToString());
        }

        [Test]
        public void CallsOverOperatorsTest()
        {
            var expression = SMExpressionParser.ParseOrThrow("$a ^ sqrt($a) * $b & log(10 * $f)");

            var sb = new StringBuilder();
            expression.Accept(new ToStringExpressionVisitor(sb));

            Assert.AreEqual("((($a:^sqrt($a:))*$b:)&log((10*$f:)))", sb.ToString());
        }

        [Test]
        public void ParensOverOperatorsTest()
        {
            var expression = SMExpressionParser.ParseOrThrow("$a ^ (sqrt($a) * ($b & log(10 * $f)))");

            var sb = new StringBuilder();
            expression.Accept(new ToStringExpressionVisitor(sb));

            Assert.AreEqual("($a:^(sqrt($a:)*($b:&log((10*$f:)))))", sb.ToString());
        }
    }
}
