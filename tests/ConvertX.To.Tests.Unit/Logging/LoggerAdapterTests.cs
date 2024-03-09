using System;
using System.Diagnostics.CodeAnalysis;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Application.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace ConvertX.To.Tests.Unit.Logging;


public class LoggerAdapterTests
{
    private const string testMessage = "This is a test message.";
    private readonly ILoggerAdapter<LoggerAdapterTests> _mockLogger;
    private readonly ILoggerAdapter<LoggerAdapterTests> _realLogger;
    private readonly NullReferenceException testException = new("example");

    public LoggerAdapterTests()
    {
        _mockLogger = Substitute.For<ILoggerAdapter<LoggerAdapterTests>>();

        var logger = new Logger<LoggerAdapterTests>(new LoggerFactory());
        _realLogger = new LoggerAdapter<LoggerAdapterTests>(logger);
    }

    [Fact]
    public void LogTrace_ShouldLogMessage_WhenInvoked()
    {
        // Act
        _mockLogger.LogTrace(testMessage);
        var action = () => _realLogger.LogTrace(testMessage);

        // Assert
        _mockLogger.Received(1).LogTrace(testMessage);
        action.Should().NotThrow();
    }

    [Fact]
    public void LogDebug_ShouldLogMessage_WhenInvoked()
    {
        // Act
        _mockLogger.LogDebug(testMessage);
        var action = () => _realLogger.LogDebug(testMessage);

        // Assert
        _mockLogger.Received(1).LogDebug(testMessage);
        action.Should().NotThrow();
    }

    [Fact]
    public void LogInformation_ShouldLogMessage_WhenInvoked()
    {
        // Act
        _mockLogger.LogInformation(testMessage);
        var action = () => _realLogger.LogInformation(testMessage);

        // Assert
        _mockLogger.Received(1).LogInformation(testMessage);
        action.Should().NotThrow();
    }

    [Fact]
    public void LogWarning_ShouldLogMessage_WhenInvoked()
    {
        // Act
        _mockLogger.LogWarning(testMessage);
        var action = () => _realLogger.LogWarning(testMessage);

        // Assert
        _mockLogger.Received(1).LogWarning(testMessage);
        action.Should().NotThrow();
    }

    [Fact]
    public void LogError_ShouldLogMessage_WhenInvoked()
    {
        // Act
        _mockLogger.LogError(testException, testMessage);
        var action = () => _realLogger.LogError(testException, testMessage);

        // Assert
        _mockLogger.Received(1).LogError(testException, testMessage);
        action.Should().NotThrow();
    }

    [Fact]
    public void LogCritical_ShouldLogMessage_WhenInvoked()
    {
        // Act
        _mockLogger.LogCritical(testException, testMessage);
        var action = () => _realLogger.LogCritical(testException, testMessage);

        // Assert
        _mockLogger.Received(1).LogCritical(testException, testMessage);
        action.Should().NotThrow();
    }
}