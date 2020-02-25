using System;
using Newtonsoft.Json;

namespace NuClear.ValidationRules.Import
{
    public static class Log
    {
        public static void Debug(string template, object arg)
        {
            Write("Debug", template, arg);
        }

        public static void Info(string template, object arg)
        {
            Write("Info", template, arg);
        }

        public static void Error(string template, object arg)
        {
            Write("Error", template, arg);
        }

        private static void Write(string level, string template, object arg)
        {
            Console.WriteLine(
                JsonConvert.SerializeObject(new {Level = level, Message = template, Data = arg}));
        }
    }
}
