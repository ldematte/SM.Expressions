using System;

namespace SM.Expressions
{
    public interface IExpression : IEquatable<IExpression>
    {
        void Accept(IExpressionVisitor visitor);
    }

    public class Identifier : IExpression
    {
        private readonly string m_id;
        private readonly string m_appName;
        private readonly bool m_isRaw;
        private readonly bool m_isNoLog;

        public Identifier(string id, string appName, bool isRaw, bool isNoLog)
        {
            m_id = id;
            m_appName = appName;
            m_isRaw = isRaw;
            m_isNoLog = isNoLog;
        }

        public bool Equals(IExpression other)
            => other is Identifier i && m_id == i.m_id && m_appName == i.m_appName && m_isNoLog == i.m_isNoLog;

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitIdentifier(m_id, m_appName, m_isRaw, m_isNoLog);
        }
    }
    
    public class Literal : IExpression
    {
        public double Value { get; }

        public Literal(double value)
        {
            Value = value;
        }

        public bool Equals(IExpression other)
            => other is Literal l && Value == l.Value;

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitLiteral(Value);
        }
    }

    public class Call : IExpression
    {
        private readonly string m_functionName;
        private readonly IExpression m_argument;

        public Call(string functionName, IExpression argument)
        {
            m_functionName = functionName;
            m_argument = argument;
        }

        public bool Equals(IExpression other)
            => other is Call c
            && m_functionName.Equals(c.m_functionName)
            && m_argument.Equals(c.m_argument);

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitFunction(m_functionName, m_argument);
        }
    }

    public enum UnaryOperatorType
    {
        Neg,
        Complement
    }

    public class UnaryOp : IExpression
    {
        public UnaryOperatorType Type { get; }
        public IExpression Expression { get; }

        public UnaryOp(UnaryOperatorType type, IExpression expression)
        {
            Type = type;
            Expression = expression;
        }

        public bool Equals(IExpression other)
            => other is UnaryOp u
            && this.Type == u.Type
            && this.Expression.Equals(u.Expression);

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitUnary(Type, Expression);
        }
    }

    public enum BinaryOperatorType
    {
        Add,
        Sub,
        Mul,
        Div, 
        Mod,
        Pow,
        And,
        Or,
        Lsh,
        Rsh
    }

    public class BinaryOp : IExpression
    {
        public BinaryOperatorType Type { get; }
        public IExpression Left { get; }
        public IExpression Right { get; }

        public BinaryOp(BinaryOperatorType type, IExpression left, IExpression right)
        {
            Type = type;
            Left = left;
            Right = right;
        }

        public bool Equals(IExpression other)
            => other is BinaryOp b
            && this.Type == b.Type
            && this.Left.Equals(b.Left)
            && this.Right.Equals(b.Right);

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitBinary(Left, Type, Right);
        }
    }
}