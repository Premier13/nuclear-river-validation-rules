using System;
using System.Diagnostics;

namespace NuClear.ValidationRules.Import.FactWriters
{
    public static class Instrumentation
    {
        public static T Do<T>(Func<T> func, string category, string name)
        {
            var timer = Stopwatch.StartNew();
            var result = func.Invoke();
            timer.Stop();
            Log.Debug(category, new {Name = name, Time = timer.Elapsed});
            return result;
        }

        public static void Do(Action func, string category, string name)
        {
            var timer = Stopwatch.StartNew();
            func.Invoke();
            timer.Stop();
            Log.Debug(category, new {Name = name, Time = timer.Elapsed});
        }
    }
}