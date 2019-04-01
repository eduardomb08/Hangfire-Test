using Hangfire;
using Microsoft.Owin.Hosting;
using Ninject;
using Serilog;
using System;
using System.Threading;

namespace HangfireTest
{
    class Program
    {
        private static IKernel _kernel = new StandardKernel();

        static void Main(string[] args)
        {
            using (WebApp.Start<Startup>("http://localhost:9000"))
            {
                RunTest();

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        private static void RunTest()
        {
            int max = 3;

            Log.Logger.Information($"Starting batch job ...");
            var bid = BatchJob.StartNew(x =>
            {
                for (int i = 0; i < max; i++)
                {
                    Log.Logger.Information($"Enqueing a new job for {i}");
                    x.Enqueue(() => DoLongWork(i));
                }
            });

            Log.Logger.Information($"Waiting for {max} jobs to finish...");
            var wid = BatchJob.AwaitBatch(bid, y =>
            {
                y.Enqueue(() => Log.Logger.Information("All jobs done."));
            });
        }

        public static void DoLongWork(int id)
        {
            Log.Logger.Information($"Starting job {id}...");
            Thread.Sleep(2000);
            Log.Logger.Information($"Job {id} finished.");
        }
    }
}
