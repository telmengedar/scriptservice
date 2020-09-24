using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ScriptService {

    /// <summary>
    /// main program containing entry point
    /// </summary>
    public class Program {

        /// <summary>
        /// main entry point
        /// </summary>
        /// <param name="args">app arguments</param>
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// creates the host builder to use
        /// </summary>
        /// <param name="args">application arguments</param>
        /// <returns>created host builder</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureAppConfiguration(builder => {
                    builder.AddJsonFile("parserconfig.json", optional: true, reloadOnChange: false);
                });
    }
}
