using System;

namespace PhilotechniaGameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Philotechnia Game Server";

            var consoleLogger = new Logger();
            Server.Start(8, 3074, consoleLogger);

            Console.ReadKey(); 
        }
    }
}
