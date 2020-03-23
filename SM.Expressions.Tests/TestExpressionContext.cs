namespace SM.Expressions.Tests
{
    internal class TestExpressionContext : IExpressionContext
    {
        public double GetValue(string identifier, string appName, bool raw, bool noLog)
        {
            if (noLog)
                return 1;
            return 10;
        }
    }
}