using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data.Common;
using Polly;
using MySql.Data.MySqlClient;

namespace WebApplication1
{

    public class Startup
    {

        public Startup(IConfiguration configuration)
        {

            Configuration = configuration;
        }

        public IConfiguration Configuration { get;

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            //services.AddMvc();
            //services.AddSingleton(sp => StartupExtensions.GetMySqlConnectionString());
            services.AddControllers();
            AddSwagger(services);
        }

        private void AddSwagger(IServiceCollection services)
        {


            services.AddSwaggerGen(options =>
            {
                var groupName = "v1";

                options.SwaggerDoc(groupName, new OpenApiInfo
                {
                    Title = $"PDN {groupName}",
                    Version = groupName,
                    Description = "PDN API",

                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Name = "Authorization",
                    Scheme = "bearer",
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri("/Home/v1/oauth", UriKind.Relative),//Linea a Cambiar
                        }
                    },
                    In = ParameterLocation.Header,

                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference{
                                Id = "Bearer", //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme,

                            }
                        },new string[] {}
                    }
                });
                options.OperationFilter<AuthorizeOperationFilter>();



            });



        }


        private class AuthorizeOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {

                var authAttributes = context.MethodInfo
                  .GetCustomAttributes(true)
                  .OfType<AuthorizeAttribute>()
                  .Distinct();

                if (authAttributes.Any())
                {


                    var jwtbearerScheme = new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    };

                    operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [ jwtbearerScheme ] = new string [] { }
                    }
                };
                }
            }

        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseCors(options =>
            {
                options.AllowAnyOrigin();
                options.AllowAnyMethod();
                options.AllowAnyHeader();

            }
            );



            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }




            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PDN API V1");
                c.DefaultModelsExpandDepth(-1);
                c.OAuthClientId("cliente");
                c.OAuthClientSecret("secret");
                c.OAuthAppName("OAuth-app");
                c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
                c.OAuthUsePkce();
            });
            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseRouting();
            app.UseAuthorization();
            app.UseCors();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }






    }


    static class StartupExtensions
    {
        public static void OpenWithRetry(this DbConnection connection) =>
            // [START cloud_sql_mysql_dotnet_ado_backoff]
            Policy
                .Handle<MySqlException>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5)
                })
                .Execute(() => connection.Open());
        // [END cloud_sql_mysql_dotnet_ado_backoff]

       
    }




}
