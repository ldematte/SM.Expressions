using System;
using System.Collections.Generic;

namespace SM.Expressions
{
    public interface IFunctionContext
    {
        Func<double, double> GetFunction(string functionName);
    }

    public class EvaluatorVisitor : IExpressionVisitor
    {
        private readonly IExpressionContext m_context;
        private readonly IParametersSymbolTable m_symbolTable;
        private readonly IFunctionContext m_functionContext;

        private readonly Stack<double> m_stack = new Stack<double>();

        public EvaluatorVisitor(IExpressionContext context, IParametersSymbolTable symbolTable, IFunctionContext functionContext)
        {
            m_context = context;
            m_symbolTable = symbolTable;
            m_functionContext = functionContext;
        }

        public double Value => m_stack.Count == 1 ? m_stack.Pop() : double.NaN;

        public void VisitIdentifier(string identifier, string appName, bool isRaw, bool isNoLog)
        {
            m_symbolTable.GetId(identifier, appName,
                id =>
                {
                    var value = m_context.GetValue(id, isRaw, isNoLog);
                    m_stack.Push(value);
                },
                () => m_stack.Push(double.NaN));
        }

        public void VisitLiteral(double value)
        {
            m_stack.Push(value);
        }

        public void VisitFunction(string functionName, IExpression argument)
        {
            argument.Accept(this);

            var argumentValue = m_stack.Pop();
            var func = m_functionContext.GetFunction(functionName);
            m_stack.Push(func(argumentValue));
        }

        public void VisitUnary(UnaryOperatorType type, IExpression expression)
        {
            expression.Accept(this);
            var value = m_stack.Pop();
            switch (type)
            {
                case UnaryOperatorType.Neg:
                    value = -value;
                    break;
                case UnaryOperatorType.Complement:
                    value = ~(int) value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            m_stack.Push(value);
        }

        public void VisitBinary(IExpression left, BinaryOperatorType type, IExpression right)
        {
            left.Accept(this);
            var leftValue = m_stack.Pop();

            right.Accept(this);
            var rightValue = m_stack.Pop();

            switch (type)
            {
                case BinaryOperatorType.Add:
                    m_stack.Push(leftValue + rightValue);
                    break;
                case BinaryOperatorType.Sub:
                    m_stack.Push(leftValue - rightValue);
                    break;
                case BinaryOperatorType.Mul:
                    m_stack.Push(leftValue * rightValue);
                    break;
                case BinaryOperatorType.Div:
                    m_stack.Push(leftValue / rightValue);
                    break;
                case BinaryOperatorType.Mod:
                    m_stack.Push(leftValue % rightValue);
                    break;
                case BinaryOperatorType.Pow:
                    m_stack.Push(Math.Pow(leftValue, rightValue));
                    break;
                case BinaryOperatorType.And:
                    m_stack.Push((int)leftValue & (int)rightValue);
                    break;
                case BinaryOperatorType.Or:
                    m_stack.Push((int)leftValue | (int)rightValue);
                    break;
                case BinaryOperatorType.Lsh:
                    m_stack.Push((int)leftValue << (int)rightValue);
                    break;
                case BinaryOperatorType.Rsh:
                    m_stack.Push((int)leftValue >> (int)rightValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}