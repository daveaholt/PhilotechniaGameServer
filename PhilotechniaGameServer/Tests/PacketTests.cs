using NUnit.Framework;
using PhilotechniaGameServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    [TestFixture]
    class PacketTests
    {
        private Packet unit;

        [SetUp]
        public void Setup()
        {
            unit = new Packet();
        }

        [Test]
        public void ShouldEncodeDecodeBool()
        {
            var b = false;
            unit.Write(b);
            var result = unit.ReadBool();
            Assert.False(b);
        }
    }
}
