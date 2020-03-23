using System;
using System.Text;

namespace SM.Expressions
{
    public class ToStringExpressionVisitor: IExpressionVisitor
    {
        private readonly StringBuilder m_stringBuilder;

        public ToStringExpressionVisitor(StringBuilder stringBuilder)
        {
            m_stringBuilder = stringBuilder;
        }

        public void VisitIdentifier(string identifier, string appName, bool isRaw, bool isNoLog)
        {
            if (isNoLog)
            {
                m_stringBuilder.Append("nolog(");
            }
            if (isRaw)
                m_stringBuilder.Append($"$${identifier}:{appName}");
            else
                m_stringBuilder.Append($"${identifier}:{appName}");
            if (isNoLog)
            {
                m_stringBuilder.Append(")");
            }
        }

        public void VisitLiteral(double value)
        {
            m_stringBuilder.Append(value);
        }

        public void VisitFunction(string functionName, IExpression argument)
        {
            m_stringBuilder.Append($"{functionName}(");
            argument.Accept(this);
            m_stringBuilder.Append(")");
        }

        public void VisitUnary(UnaryOperatorType type, IExpression expression)
        {
            switch (type)
            {
                case UnaryOperatorType.Neg:
                    m_stringBuilder.Append("-(");
                    break;
                case UnaryOperatorType.Complement:
                    m_stringBuilder.Append("~(");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            
            expression.Accept(this);
            m_stringBuilder.Append(")");
        }

        public void VisitBinary(IExpression left, BinaryOperatorType type, IExpression right)
        {
            m_stringBuilder.Append("(");
            left.Accept(this);
            switch (type)
            {
                case BinaryOperatorType.Add:
                    m_stringBuilder.Append("+");
                    break;
                case BinaryOperatorType.Sub:
                    m_stringBuilder.Append("-");
                    break;
                case BinaryOperatorType.Mul:
                    m_stringBuilder.Append("*");
                    break;
                case BinaryOperatorType.Div:
                    m_stringBuilder.Append("/");
                    break;
                case BinaryOperatorType.Mod:
                    m_stringBuilder.Append("%");
                    break;
                case BinaryOperatorType.Pow:
                    m_stringBuilder.Append("^");
                    break;
                case BinaryOperatorType.And:
                    m_stringBuilder.Append("&");
                    break;
                case BinaryOperatorType.Or:
                    m_stringBuilder.Append("|");
                    break;
                case BinaryOperatorType.Lsh:
                    m_stringBuilder.Append("<<");
                    break;
                case BinaryOperatorType.Rsh:
                    m_stringBuilder.Append(">>");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            right.Accept(this);
            m_stringBuilder.Append(")");
        }
    }
}