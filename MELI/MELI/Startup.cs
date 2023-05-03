using MELI.DataAccess;
using MELI.DataAccess.Interfaces;
using MELI.Services;
using MELI.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace MELI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MELI", Version = "v1" });
            });

            services.AddOptions();
            services.AddTransient<IControlDePeticionesService, ControlDePeticionesService>();
            services.AddTransient<IEstadisticasUsoService, EstadisticasUsoService>();
            services.AddTransient<IControlRepository, ControlRepository>();
            services.AddTransient<IControlRepository, IpControlRepository>();
            services.AddTransient<IControlRepository, EndpointControlRepository>();
            services.AddTransient<IRequestRepository, RequestRepository>();
            services.AddTransient<IProxyService, ProxyService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MELI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
