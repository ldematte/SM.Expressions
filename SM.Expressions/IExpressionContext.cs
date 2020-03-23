namespace SM.Expressions
{
    public interface IExpressionContext
    {
        double GetValue(string identifier, string appName, bool raw, bool noLog);
    }
}