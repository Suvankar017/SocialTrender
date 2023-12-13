using System;

namespace SocialTrender
{
    public static class Log
    {
        private const ConsoleColor s_InfoColor = ConsoleColor.White;
        private const ConsoleColor s_WarnColor = ConsoleColor.DarkYellow;
        private const ConsoleColor s_ErrorColor = ConsoleColor.Red;
        private const ConsoleColor s_SuccessColor = ConsoleColor.Green;
        private const ConsoleColor s_LinkColor = ConsoleColor.Blue;

        public static void Info(string value)
        {
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = s_InfoColor;
            Console.WriteLine(value);
            Console.ForegroundColor = prevColor;
        }

        public static void Info(object value)
        {
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = s_InfoColor;
            Console.WriteLine(value);
            Console.ForegroundColor = prevColor;
        }

        public static void Warn(string value)
        {
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = s_WarnColor;
            Console.WriteLine(value);
            Console.ForegroundColor = prevColor;
        }

        public static void Warn(object value)
        {
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = s_WarnColor;
            Console.WriteLine(value);
            Console.ForegroundColor = prevColor;
        }

        public static void Error(string value)
        {
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = s_ErrorColor;
            Console.WriteLine(value);
            Console.ForegroundColor = prevColor;
        }

        public static void Error(object value)
        {
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = s_ErrorColor;
            Console.WriteLine(value);
            Console.ForegroundColor = prevColor;
        }

        public static void Success(string value)
        {
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = s_SuccessColor;
            Console.WriteLine(value);
            Console.ForegroundColor = prevColor;
        }

        public static void Success(object value)
        {
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = s_SuccessColor;
            Console.WriteLine(value);
            Console.ForegroundColor = prevColor;
        }

        public static void Link(string value)
        {
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = s_LinkColor;
            Console.WriteLine(value);
            Console.ForegroundColor = prevColor;
        }

        public static void Link(object value)
        {
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = s_LinkColor;
            Console.WriteLine(value);
            Console.ForegroundColor = prevColor;
        }
    }
}
