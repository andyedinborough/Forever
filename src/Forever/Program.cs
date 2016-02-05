using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Forever
{

    public class Program
    {
        private static int _remain;
        public static void Main(string[] args)
        {
            TestForeverThread();
            //TestForeverThreadPool();
            //TestForeverTask();
        }

        static void Sample()
        {
            var proc = System.Diagnostics.Process.GetCurrentProcess();
            var mb = proc.PrivateMemorySize64 / 1024.0 / 1024.0;
            var cpu = proc.PrivilegedProcessorTime.TotalMilliseconds;
            Console.WriteLine($"{mb:0.00}mb\t{cpu}ms");
        }

        static bool Callback()
        {
            _remain--;
            Sample();
            return _remain > 0;
        }

        public static void TestForeverThread() => Test(() => new ForeverThread(Callback));

        public static void TestForeverThreadPool() => Test(() => new ForeverThreadPool(Callback));

        public static void TestForeverTask() => Test(() => new ForeverThread(Callback));

        public static void Test(Action action)
        {
            _remain = 100000;
            var mre = new ManualResetEventSlim(false);
            action();
            while (_remain > 0) mre.Wait(100);
        }
    }
}
