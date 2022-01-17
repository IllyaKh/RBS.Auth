using System;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Moq;

namespace RBS.Auth.UnitTests.TestExtensions;

public static class LoggerExtensions<TType> where TType : class
{
    public static Expression<Action<ILogger<TType>>> VerifyLogging(string message, LogLevel level)
    {
        Func<object, Type, bool> state = (v, t) => string.Compare(v.ToString(), message, StringComparison.Ordinal) == 0;

        return x => x.Log(
            It.Is<LogLevel>(l => l == level),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => state(v, t)),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!);
    }
}