using Microsoft.Extensions.DependencyInjection;
using Moq;
using Schemas;

namespace BigQuery.EntityFrameworkCore.UnitTests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBqContext<BqTestContext>(x => x.ProjectId = "moonlite");
            services.AddSingleton(x => new Mock<IExecuteQuery>());
            services.AddTransient(x => x.GetRequiredService<Mock<IExecuteQuery>>().Object);
        }
    }
}
