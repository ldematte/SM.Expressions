using System;
using System.Reflection;

namespace SM.Expressions
{
    public class DefaultCallContext : ICallContext
    {
        public MethodInfo GetFunction(string functionName)
        {
            switch (functionName)
            {
                case "log" : return typeof(Math).GetMethod("Log10", new[] { typeof(double) });
                case "ln"  : return typeof(Math).GetMethod("Log", new[] { typeof(double) });
                case "abs" : return typeof(Math).GetMethod("Abs", new[] { typeof(double) });
                case "sin" : return typeof(Math).GetMethod("Sin", new[] { typeof(double) });
                case "asin": return typeof(Math).GetMethod("Asin", new[] { typeof(double) });
                case "sinh": return typeof(Math).GetMethod("Sinh", new[] { typeof(double) });
                case "cos" : return typeof(Math).GetMethod("Cos", new[] { typeof(double) });
                case "acos": return typeof(Math).GetMethod("Acos", new[] { typeof(double) });
                case "cosh": return typeof(Math).GetMethod("Cosh", new[] { typeof(double) });
                case "tan" : return typeof(Math).GetMethod("Tan", new[] { typeof(double) });
                case "atan": return typeof(Math).GetMethod("Atan", new[] { typeof(double) });
                case "tanh": return typeof(Math).GetMethod("Tanh", new[] { typeof(double) });
                case "sqrt": return typeof(Math).GetMethod("Sqrt", new[] { typeof(double) });
                case "exp" : return typeof(Math).GetMethod("Exp", new[] { typeof(double) });
                default:
                    throw new ArgumentOutOfRangeException(nameof(functionName), functionName);
            }
        }
    }
}