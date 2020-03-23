using System.Collections.Generic;

namespace SM.Expressions
{
    public class IdentifiersVisitor : IExpressionVisitor
    {
        private readonly IDictionary<string, SMIdentifier> m_identifiers;

        public IdentifiersVisitor(IDictionary<string, SMIdentifier> identifiers)
        {
            m_identifiers = identifiers;
        }

        public void VisitIdentifier(string id, string appName, bool isRaw, bool isNoLog)
        {
            var fqn = string.IsNullOrEmpty(appName) ? id : $"{id}:{appName}";
            
            if (m_identifiers.TryGetValue(fqn, out var identifier))
            {
                // Normal wins over NoLog
                identifier.IsNoLog &= isNoLog;
            }
            else
            {
                m_identifiers.Add(id, new SMIdentifier(fqn, appName, isNoLog));
            }
        }

        public void VisitLiteral(double value)
        {
        }

        public void VisitFunction(string functionName, IExpression arguments)
        {
            arguments.Accept(this);
        }

        public void VisitUnary(UnaryOperatorType type, IExpression expression)
        {
            expression.Accept(this);
        }

        public void VisitBinary(IExpression left, BinaryOperatorType type, IExpression right)
        {
            left.Accept(this);
            right.Accept(this);
        }
    }
}