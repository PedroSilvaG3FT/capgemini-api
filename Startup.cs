using capgemini_api.Services;
using capgemini_api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace capgemini_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            var connection = Configuration["ConexaoSqlite:SqliteConnectionString"];
            services.AddDbContext<ContatoContexto>(options =>
                options.UseSqlite(connection)
            );
            services.AddCors(options =>
            {
                options.AddPolicy("AllowDev",
                 builder => builder.WithOrigins("*").AllowAnyHeader()
                    .AllowAnyMethod()
                 );
            });

            services = ContainerInjecaoDependencia(services);

        }

        private IServiceCollection ContainerInjecaoDependencia(IServiceCollection services)
        {
            services.AddScoped<ImportacaoService>();

            return services;
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowDev");

            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
