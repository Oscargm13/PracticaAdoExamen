using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DEPARTAMENTOSEMPLEADOSADO.Helppers
{
    public class HelpperConfiguration
    {
        public static string GetConnectionString()
        {
            ConfigurationBuilder builder =
            new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("C:\\Users\\oskit\\Entorno_desarrollo\\Master\\NetCore\\ADONETCORE\\ADONETCORE\\appSettings.json", false, true);
            IConfigurationRoot configuration = builder.Build();
            string connectionString =
            configuration.GetConnectionString("SqlTajamar");
            return connectionString;
        }
    }
}
