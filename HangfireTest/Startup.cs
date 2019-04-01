using Hangfire;
using Microsoft.Owin;
using Ninject;
using Owin;
using Serilog;
using Serilog.Core;

[assembly: OwinStartup(typeof(HangfireTest.Startup))]

namespace HangfireTest
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            GlobalConfiguration.Configuration.UseSqlServerStorage("_DBCONNECT.LOCAL_HANGFIRESTORAGE");
            GlobalConfiguration.Configuration.UseBatches();

            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger = logger;

            var kernel = new StandardKernel();
            kernel.Bind<ILogger>().To<Logger>();
            kernel.Bind<Logger>().ToConstant(logger);

            GlobalConfiguration.Configuration.UseNinjectActivator(kernel);

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}
