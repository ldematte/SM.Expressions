using System;

namespace SM.Expressions
{
    public class DefaultFunctionContext : IFunctionContext
    {
        public Func<double, double> GetFunction(string functionName)
        {
            switch (functionName)
            {
                case "log": return Math.Log10;
                case "ln" : return Math.Log;
                case "abs": return Math.Abs;
                case "sin": return Math.Sin;
                case "asin": return Math.Asin;
                case "sinh": return Math.Sinh;
                case "cos": return Math.Cos;
                case "acos": return Math.Acos;
                case "cosh": return Math.Cosh;
                case "tan": return Math.Tan;
                case "atan": return Math.Atan;
                case "tanh": return Math.Tanh;
                case "sqrt": return Math.Sqrt;
                case "exp": return Math.Exp;
                default:
                    throw new ArgumentOutOfRangeException(nameof(functionName), functionName);
            }
        }
    }
}