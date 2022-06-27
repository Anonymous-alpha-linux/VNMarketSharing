using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

using AdsMarketSharing.Services.Collaborator;
using AdsMarketSharing.Data;
using AdsMarketSharing.Interfaces;
using AdsMarketSharing.Repositories;

using Swashbuckle.AspNetCore.Filters;
using AdsMarketSharing.Services.Email;
using AdsMarketSharing.Models.Email;
using AdsMarketSharing.Models.Token;
using AdsMarketSharing.Enum;

namespace AdsMarketSharing
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
            services.AddOptions();
            services.AddDbContext<SQLExpressContext>(x => x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
          /*  services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<SQLExpressContext>();*/
            services.AddControllers();
            services.AddAutoMapper(typeof(Startup).Assembly);
            // services.AddCors();
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "V1",
                    Title = "AdsMarketSharing API",
                    Description = "API"
                });
                s.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization Header using the Bearer scheme. Ex: \"bearer {your token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                s.OperationFilter<SecurityRequirementsOperationFilter>();
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Specify the key used to sign the token:
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    // Clock skew compensates for server time drift.
                    // We recommend 5 minutes or less:
                    ClockSkew = TimeSpan.Zero,
                    // Ensure the token hasn't expired:
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    // Ensure the token audience matches our audience value (default true):
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    RequireSignedTokens= true
                };
            });
        
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // My services
            services.AddScoped<ICollaborator, Collaborator>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IMailService, MailService>()
                .Configure<EmailConfiguration>(Configuration.GetSection("AppSettings:EmailSettings"));
            services.AddScoped<IToken, TokenRepository>();                
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseCors(builder => builder.AllowAnyOrigin().WithMethods("GET", "POST").AllowAnyHeader());
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AdsMarketSharing API");
                c.RoutePrefix = String.Empty;
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
