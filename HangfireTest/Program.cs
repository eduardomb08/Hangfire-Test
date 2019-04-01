using Hangfire;
using Microsoft.Owin.Hosting;
using Ninject;
using Serilog;
using Serilog.Core;
using System;
using System.Diagnostics;
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
                //RecurringJob.AddOrUpdate(() => Console.WriteLine("Hangfire Works"), Cron.Minutely);
                RunTest();

                Console.WriteLine("Hangfire on");
                Console.ReadKey();
            }

            //SetupLogger();
            //SetupHangfire();

            //var act = CreateActivator();
            //var logger = act.ActivateJob(typeof(Logger));

            //using (var server = new BackgroundJobServer())
            //{
            //    RunTest();

            //    Console.WriteLine("Press any key to exit...");
            //    Console.ReadKey();
            //}
        }

        //private static NinjectJobActivator CreateActivator()
        //{
        //    return new NinjectJobActivator(_kernel);
        //}

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

        //private static void SetupHangfire()
        //{
        //    _kernel.Bind<ILogger>().To<Logger>();
        //    _kernel.Bind<Logger>().ToConstant(GetLogger());

        //    GlobalConfiguration.Configuration.UseNinjectActivator(_kernel);

        //    GlobalConfiguration.Configuration.UseSqlServerStorage("_DBCONNECT.LOCAL_HANGFIRESTORAGE");
        //    GlobalConfiguration.Configuration.UseBatches();
        //}

        //private static void SetupLogger()
        //{
        //    Log.Logger = GetLogger();
        //}

        //private static Logger GetLogger()
        //{
        //    return new LoggerConfiguration()
        //        .WriteTo.Console()
        //        .WriteTo.File($@"C:\Users\cruzedu\Downloads\hangfire_{Process.GetCurrentProcess().Id}.txt")
        //        .CreateLogger();
        //}

        public static void DoLongWork(int id)
        {
            Log.Logger.Information($"Starting job {id}...");
            Thread.Sleep(2000);
            Log.Logger.Information($"Job {id} finished.");
        }
    }
}
