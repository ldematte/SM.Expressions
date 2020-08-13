# SM.Expressions
Expression parser, evaluator, compiler.

Expressions over channels (streams of data, AKA signals, logging parameters or "parameters" for short) are parsed and then used in the VirtualChannels project to provide support for virtual (computed) channels.
Based on how the expression is written, virtual channels are created in the most optimized way:
- Alias (direct mapping of a real (i.e. present in the physical stream) channel to a new name)
- SlowRow (computation triggered by "unfrequent data", i.e. point-value change alone)
- SingleLoggedParameter (expression evaluation on a single (time,value) tuple)
- MultiLoggedParameters (expression evaluation with buffered values, on the combined parameter coverage)

As it is evident from this short explanation, concepts are lifted from the world of "loggers", i.e. devices that produce a stream of data. Similar to IoT as a bottom line, but unlike IoT devices, the frequency and amount of data is several orders of magnitude more (up to frequencies in the KHz range, and to thousands of signals).
However, this PoC is generally applicable to several stream-based processing scenarios and is in no way linked to a particular device or product.

## Evolutions (WIP)
- Acting on chunks instead of single values to allow vectorization
- Use intrinsics (https://devblogs.microsoft.com/dotnet/hardware-intrinsics-in-net-core/) instead of basic math operations whenever possible
