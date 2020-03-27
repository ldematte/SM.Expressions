# SM.Expressions
SM expression parser, evaluator, compiler.

Parsed expressions are then used in VirtualChannels to provide support for virtual MES parameters.
Based on the expression, virtual channels are created in the most optimized way:
- Alias (direct mapping of a identifier:application to a new name)
- SlowRow (computation triggered by slowrow value change alone)
- SingleLoggedParameter (expression evaluation on a single time,value tuple)
- MultiLoggedParameters (expression evaluation with buffered values, on the combined parameter coverage)

