using FleetManager.Infrastructure;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;

namespace FleetManager.API
{
    public class Startup
    {
        private const int BODY_LOG_LIMIT = 4096;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpLogging(httpLogging =>
            {
                httpLogging.LoggingFields = HttpLoggingFields.All;
                httpLogging.RequestHeaders.Add("Request-Header-Demo");
                httpLogging.ResponseHeaders.Add("Response-Header-Demo");
                httpLogging.MediaTypeOptions.
                AddText("application/javascript");
                httpLogging.RequestBodyLogLimit = BODY_LOG_LIMIT;
                httpLogging.ResponseBodyLogLimit = BODY_LOG_LIMIT;
            });

            services.AddDbContext<FleetDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers();
            services.AddHealthChecks();
            services.AddHttpContextAccessor();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseHttpLogging();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(x => x.WithOrigins(Configuration.GetSection("AllowedOrigins").Get<string>().Split(","))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("Content-Disposition"));

            app.UseAuthorization();

            app.UseHealthChecks("/");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
