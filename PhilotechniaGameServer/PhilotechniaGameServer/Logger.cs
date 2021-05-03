using System;
using System.Collections.Generic;
using System.Text;

namespace PhilotechniaGameServer
{
    public interface ILogger
    {
        void WriteLine(string value);
    }
    public class Logger : ILogger
    {

        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }
    }
}
