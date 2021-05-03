using NUnit.Framework;
using PhilotechniaGameServer;
using System;

namespace Tests
{
    public class GameServerTests
    {
        private Server unit;
        private ILogger logger;

        [SetUp]
        public void Setup()
        {
            logger = new ConsoleLogger();
        }

        [Test]
        public void ShouldStartServer()
        {
            Server.Start(4, 3074, logger);
            Assert.Pass();
        }

        public class ConsoleLogger : ILogger
        {
            public void WriteLine(string value)
            {
                Console.WriteLine(value);
            }
        }
    }
}