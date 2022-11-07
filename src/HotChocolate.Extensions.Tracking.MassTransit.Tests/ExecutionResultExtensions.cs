using HotChocolate;
using HotChocolate.Execution;

namespace HotChocolate.Extensions.Tracking.MassTransit.Tests;

internal static class ExecutionResultExtensions
{
    internal static string ToMinifiedJson(this IExecutionResult s)
    {
        return s.ToJson()
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty)
            .Replace("\t", string.Empty)
            .Replace(" ", string.Empty);
    }
}
