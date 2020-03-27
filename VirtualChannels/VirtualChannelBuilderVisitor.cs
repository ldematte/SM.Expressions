using System;
using System.Collections.Generic;
using SM.Expressions;

namespace VirtualChannels
{
    internal class VirtualChannelBuilderVisitor : IExpressionVisitor
    {
        private readonly IParametersSymbolTable m_symbolTable;
        private readonly HashSet<int> m_loggedParameters = new HashSet<int>();
        private readonly HashSet<int> m_nologParameters = new HashSet<int>();

        private bool m_complexExpression;

        public VirtualChannelBuilderVisitor(IParametersSymbolTable symbolTable)
        {
            m_symbolTable = symbolTable;
        }

        public void VisitIdentifier(string identifier, string appName, bool isRaw, bool isNoLog)
        {
            m_symbolTable.GetId(identifier, appName, 
                id =>
                {
                    if (isNoLog)
                    {
                        m_nologParameters.Add(id);
                    }
                    else
                    {
                        m_loggedParameters.Add(id);
                    }
                }, 
                () => throw new Exception($"Unknown identifier {identifier}:{appName}"));
        }

        public void VisitLiteral(double value)
        {
        }

        public void VisitFunction(string functionName, IExpression arguments)
        {
            m_complexExpression = true;
        }

        public void VisitUnary(UnaryOperatorType type, IExpression expression)
        {
            m_complexExpression = true;
        }

        public void VisitBinary(IExpression left, BinaryOperatorType type, IExpression right)
        {
            m_complexExpression = true;
        }

        internal bool IsAlias()
        {
            return m_complexExpression == false && m_loggedParameters.Count == 1;
        }

        internal bool IsSlowRow()
        {
            return m_loggedParameters.Count == 0;
        }

        internal IEnumerable<int> LoggedParameters => m_loggedParameters;
        internal IEnumerable<int> NoLogParameters => m_nologParameters;
    }
}