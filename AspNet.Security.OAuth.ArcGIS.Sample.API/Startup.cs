using AspNet.Security.OAuth.ArcGIS.Sample.API.Infrastructure.OpenApi;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AspNet.Security.OAuth.ArcGIS.Sample.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        private IHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(ArcGISAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => options.ForwardChallenge = ArcGISAuthenticationDefaults.AuthenticationScheme)
                .AddArcGIS(options =>
                {
                    options.ClientId = Configuration["Credentials:ClientId"];
                    options.ClientSecret = Configuration["Credentials:ClientSecret"];
                    options.AuthorizationEndpoint = Configuration["Credentials:AuthorizationEndpoint"];
                    options.TokenEndpoint = Configuration["Credentials:TokenEndpoint"];
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.SaveTokens = true;
                });

            services
               .AddCors(options =>
               {
                   options.AddPolicy("CorsPolicy",
                       builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                   );
               })               
               .AddApiVersioning(version =>
               {
                   version.DefaultApiVersion = new ApiVersion(1, 0);
                   version.ReportApiVersions = true;
                   version.AssumeDefaultVersionWhenUnspecified = true;
                   version.UseApiBehavior = true;
               })
               .AddProblemDetails(configure =>
               {
                   configure.IncludeExceptionDetails = (_, __) => Environment.EnvironmentName == "Development";
               })
               .Configure<ApiBehaviorOptions>(options =>
               {
                   options.SuppressModelStateInvalidFilter = false;
                   options.SuppressInferBindingSourcesForParameters = false;

                   options.InvalidModelStateResponseFactory = context =>
                   {
                       var problemDetails = new ValidationProblemDetails(context.ModelState)
                       {
                           Instance = context.HttpContext.Request.Path,
                           Status = StatusCodes.Status400BadRequest,
                           Type = $"https://httpstatuses.com/400",
                           Detail = "Please refer to the errors property for additional details."
                       };
                       return new BadRequestObjectResult(problemDetails)
                       {
                           ContentTypes =
                         {
                                "application/problem+json",
                                "application/problem+xml"
                         }
                       };
                   };
               })
               .AddVersionedApiExplorer()
               .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>()
               .AddSwaggerGen();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersion)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseProblemDetails();

            app.UseSwagger()
                .UseSwaggerUI(options =>
                {
                    options.RoutePrefix = string.Empty;
                    foreach (var description in apiVersion.ApiVersionDescriptions)
                    {
                        string swaggerJsonBasePath = string.IsNullOrWhiteSpace(options.RoutePrefix) ? "." : "..";
                        options.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/{description.GroupName}/swagger.json", description.GroupName);
                    }

                    options.OAuthClientId(Configuration["Credentials:ClientId"]);
                    options.OAuthClientSecret(Configuration["Credentials:ClientSecret"]);
                    options.OAuthAppName("SAMPLE API - Swagger");
                    options.OAuthScopeSeparator(" ");
                });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
