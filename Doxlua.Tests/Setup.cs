namespace Doxlua.Tests
{
    using Doxlua.Lexer;
    using Doxlua.VM;
    using Xunit;

    using NLog;
    using NLog.Config;
    using NLog.Targets;

    public class LoggingFixture
    {
        static LoggingFixture()
        {
            var config = new LoggingConfiguration();

            var logfile = new FileTarget("logfile") { FileName = "../../../../logs.txt" };
            var logconsole = new ConsoleTarget("logconsole");

            config.AddRule(LogLevel.Warn, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            LogManager.Configuration = config;
        }

        // Optional: expose the logger for use in tests
        public Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    }

    [CollectionDefinition("BasicSeries")]
    public class LoggingCollection : ICollectionFixture<LoggingFixture>
    {
        // No code needed here; xUnit uses this to discover the fixture.
    }
}