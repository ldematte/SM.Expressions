using System;
using SM.Expressions;

namespace VirtualChannels
{
    internal class MultiValueWithSlowRowExpressionContext<TTime> : IExpressionContext
    {
        public MultiValueWithSlowRowExpressionContext(ISlowRowStorage<TTime> slowRowStorage)
        {
            throw new NotImplementedException();
        }

        public double GetValue(string identifier, string appName, bool raw, bool noLog)
        {
            throw new NotImplementedException();
        }
    }
}