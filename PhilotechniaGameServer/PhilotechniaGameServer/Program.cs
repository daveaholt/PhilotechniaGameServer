using System;

namespace PhilotechniaGameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Philotechnia Game Server";
            Server.Start(8, 3074);
            Console.ReadKey(); 
        }
    }
}
