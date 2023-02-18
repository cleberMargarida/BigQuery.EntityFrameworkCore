using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Schemas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigQuery.EntityFrameworkCore.FuncionalTests
{
    public class Startup
    {
        IConfiguration Configuration { get; } = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .Build();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBqContext<BqTestContext>(x =>
            {
                x.ProjectId = "moonlit-text-367106";
                x.GoogleCredential = GoogleCredential.FromJson(Configuration["google_auth"]);
            });
        }
    }
}
