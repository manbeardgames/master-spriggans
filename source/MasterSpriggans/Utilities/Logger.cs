using System;

namespace MasterSpriggans.Utils
{
    public enum LogType { Message, Warning, Error }

    public static class Logger
    {
        public static void Message(string message) => Log(message, LogType.Message);
        public static void Warning(string message) => Log(message, LogType.Warning);
        public static void Error(string message) => Log(message, LogType.Error);

        public static void Log(string message, LogType type)
        {
            switch (type)
            {
                case LogType.Message:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                default:
                    throw new Exception("Unknown log type");
            }

            Console.Write($"[{DateTime.Now.ToShortTimeString()}]  ");
            Console.Write($"[{type.ToString().ToUpper()}]  ");
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}