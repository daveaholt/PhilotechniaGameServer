using System;
using System.Threading;

namespace PhilotechniaGameServer
{
    class Program
    {
        private static bool isRunning = false;

        static void Main(string[] args)
        {
            Console.Title = "Philotechnia Game Server";
            isRunning = true;

            var consoleLogger = new Logger();

            var mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Server.Start(8, 3074, consoleLogger);

        }

        private static void MainThread()
        {
            var consoleLogger = new Logger();
            consoleLogger.WriteLine($"Main thread started. Runnin at {Constants.TICKS_PER_SEC} ticks per second");
            var nextLoop = DateTime.Now;

            while (isRunning)
            {
                while(nextLoop < DateTime.Now)
                {
                    GameLogic.Update();
                    nextLoop = nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    if(nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}
