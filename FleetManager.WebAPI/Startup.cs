using FleetManager.Domain.Repositories;
using FleetManager.Domain.Services;
using FleetManager.Infrastructure;
using FleetManager.Infrastructure.Repositories;
using FleetManager.Infrastructure.Services;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


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

            services.AddTransient(typeof(IVehicleRepository), typeof(VehicleRepository));
            services.AddTransient(typeof(IChassiRepository), typeof(ChassiRepository));
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IChassiService, ChassiService>();


            services.AddAuthentication("Bearer")
                    .AddJwtBearer("Bearer", options =>
                    {
                        options.Authority = $"https://login.microsoftonline.com/{Configuration["AzureAd:TenantId"]}/v2.0";
                        options.Audience = Configuration["AzureAd:ClientId"];
                        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidIssuers = new[]
                            {
                                $"https://sts.windows.net/{Configuration["AzureAd:TenantId"]}/",
                                $"https://login.microsoftonline.com/{Configuration["AzureAd:TenantId"]}/v2.0"
                            }
                        };
                    });

            services.AddAuthorization();

            services.AddDbContext<FleetDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers();
            services.AddHealthChecks();
            services.AddHttpContextAccessor();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FleetManager API", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://login.microsoftonline.com/5f3d3de5-5d5f-45b9-bccf-6b8d5aea1a03/oauth2/v2.0/authorize"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "OpenID Connect scope" },
                                { "profile", "Profile scope" },
                                { $"{Configuration["AzureAd:ClientId"]}/.default", "Access FleetManager API" }
                            }
                        }
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            }
                        },
                        new[] { "openid", "profile", $"{Configuration["AzureAd:ClientId"]}/.default" }
                    }
                });
            });
        }


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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHealthChecks("/");
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FleetManager API v1");
                c.OAuthClientId(Configuration["AzureAd:ClientId"]);              
                c.OAuthScopeSeparator(" ");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
