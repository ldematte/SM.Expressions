namespace SM.Expressions
{
    public interface IExpressionContext
    {
        double GetValue(int parameterId, bool raw, bool noLog);
    }
}