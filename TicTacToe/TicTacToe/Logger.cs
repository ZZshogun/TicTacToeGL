using System;

namespace TicTacToe
{
    public static class Logger
    {
        private static int LogID = 1;

        public static bool LogEnabled = true;
        private static readonly Queue<string> logs = new();

        public static void AddFlush<T>(T message)
        {
            if (!LogEnabled) return;
            logs.Enqueue(message?.ToString() ?? "");
        }

        public static void Flush(string endline = "\n")
        {
            if (logs.Count == 0 || !LogEnabled) return;
            string tmp = "";
            while (logs.Count > 1) tmp += logs.Dequeue() + endline;
            tmp += logs.Dequeue();
            Log(tmp);
        }

        public static void Log<T>(T message)
        {
            if (!LogEnabled) return;
            Console.WriteLine($"{LogID++} LOG | {message}");
            Console.WriteLine(@"-----------------------------------------------");
        }

        public static void LogWarn<T>(T message)
        {
            if (!LogEnabled) return;
            Console.WriteLine($"{LogID++} WARNING | {message}");
            Console.WriteLine(@"-----------------------------------------------");
        }

        public static void LogError<T>(T message)
        {
            if (!LogEnabled) return;
            Console.WriteLine($"{LogID++} ERROR | {message}");
            Console.WriteLine(@"-----------------------------------------------");
        }
    }
}
