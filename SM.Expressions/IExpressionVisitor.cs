namespace SM.Expressions
{
    public interface IExpressionVisitor
    {
        void VisitIdentifier(string identifier, string appName, bool isRaw, bool isNoLog);
        void VisitLiteral(double value);
        void VisitFunction(string functionName, IExpression arguments);
        void VisitUnary(UnaryOperatorType type, IExpression expression);
        void VisitBinary(IExpression left, BinaryOperatorType type, IExpression right);
    }
}