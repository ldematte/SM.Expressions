using System;

namespace SM.Expressions
{
    public interface IParametersSymbolTable
    {
        void GetId(string identifier, string appName, Action<int> found, Action notFound);
    }
}