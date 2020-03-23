using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SM.Expressions
{
    public interface ICallContext
    {
        MethodInfo GetFunction(string functionName);
    }

    public class CompileVisitor : IExpressionVisitor
    {
        private readonly IExpressionContext m_context;
        private readonly ICallContext m_callContext;

        private readonly Stack<Expression> m_stack = new Stack<Expression>();

        public CompileVisitor(IExpressionContext context, ICallContext callContext)
        {
            m_context = context;
            m_callContext = callContext;
        }

        public Func<double> GetCompiledExpression()
        {
            var lambdaExpression = Expression.Lambda<Func<double>>(m_stack.Pop());
            return lambdaExpression.Compile(DebugInfoGenerator.CreatePdbGenerator());
        }

        public void VisitIdentifier(string identifier, string appName, bool isRaw, bool isNoLog)
        {
            m_stack.Push(Expression.Call(Expression.Constant(m_context), m_context.GetType().GetMethod("GetValue"),
                new [] { Expression.Constant(identifier), Expression.Constant(appName), Expression.Constant(isRaw), Expression.Constant(isNoLog) }));
        }

        public void VisitLiteral(double value)
        {
            m_stack.Push(Expression.Constant(value));
        }

        public void VisitFunction(string functionName, IExpression argument)
        {
            argument.Accept(this);

            var argumentExpression = m_stack.Pop();
            var func = m_callContext.GetFunction(functionName);

            m_stack.Push(Expression.Call(func, argumentExpression));
        }

        public void VisitUnary(UnaryOperatorType type, IExpression expr)
        {
            expr.Accept(this);
            var expression = m_stack.Pop();

            switch (type)
            {
                case UnaryOperatorType.Neg:
                    m_stack.Push(Expression.Negate(expression));
                    break;
                case UnaryOperatorType.Complement:
                    m_stack.Push(Expression.Not(expression));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void VisitBinary(IExpression left, BinaryOperatorType type, IExpression right)
        {
            left.Accept(this);
            var leftExpression = m_stack.Pop();

            right.Accept(this);
            var rightExpression = m_stack.Pop();

            switch (type)
            {
                case BinaryOperatorType.Add:
                    m_stack.Push(Expression.Add(leftExpression, rightExpression));
                    break;
                case BinaryOperatorType.Sub:
                    m_stack.Push(Expression.Subtract(leftExpression, rightExpression));
                    break;
                case BinaryOperatorType.Mul:
                    m_stack.Push(Expression.Multiply(leftExpression, rightExpression));
                    break;
                case BinaryOperatorType.Div:
                    m_stack.Push(Expression.Divide(leftExpression, rightExpression));
                    break;
                case BinaryOperatorType.Mod:
                    m_stack.Push(Expression.Modulo(leftExpression, rightExpression));
                    break;
                case BinaryOperatorType.Pow:
                    m_stack.Push(Expression.Call(typeof(Math).GetMethod("Pow", new[] {typeof(double)}),
                        Expression.Convert(leftExpression, typeof(double)), Expression.Convert(rightExpression, typeof(double))));
                    break;
                case BinaryOperatorType.And:
                    m_stack.Push(Expression.And(Expression.Convert(leftExpression, typeof(int)), Expression.Convert(rightExpression, typeof(int))));
                    break;
                case BinaryOperatorType.Or:
                    m_stack.Push(Expression.Or(Expression.Convert(leftExpression, typeof(int)), Expression.Convert(rightExpression, typeof(int))));
                    break;
                case BinaryOperatorType.Lsh:
                    m_stack.Push(Expression.LeftShift(Expression.Convert(leftExpression, typeof(int)), Expression.Convert(rightExpression, typeof(int))));
                    break;
                case BinaryOperatorType.Rsh:
                    m_stack.Push(Expression.RightShift(Expression.Convert(leftExpression, typeof(int)), Expression.Convert(rightExpression, typeof(int))));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}