using AnchorTest.Properties;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnchorTest
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ConfigureServices();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddScoped<ILogger, Logger>();
            services.AddScoped<IWebcamService<Ch1Settings>, WebcamService<Ch1Settings>>();
            services.AddScoped<IWebcamSettings<Ch1Settings>, Ch1Settings>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
