using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using System;
using System.Text;
using AdsMarketSharing.Data;
using AdsMarketSharing.Interfaces;
using AdsMarketSharing.Repositories;
using AdsMarketSharing.Services.FileUpload;
using AdsMarketSharing.Services.Email;
using AdsMarketSharing.Models.Email;
using Swashbuckle.AspNetCore.Filters;
using AdsMarketSharing.Hubs;
using AdsMarketSharing.Models.Payment;
using AdsMarketSharing.Services.Payment;
using AdsMarketSharing.Entities;

using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

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

            //services.AddIdentity().AddEntityFrameworkStores<SQLExpressContext>();
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);
            });
            //services.AddControllersWithViews();

            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowAPIRequestIO", builder => {
                    builder.WithOrigins("http://localhost:3000", "https://react-vnmarketsharing.netlify.app");
                    builder.AllowAnyHeader();
                    builder.AllowCredentials();
                    builder.AllowAnyMethod();
                });
 
            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
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
                        RequireSignedTokens = true
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            bool isHub = !string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/hubs"));
                            if (isHub)
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        { 
                            return Task.CompletedTask;   
                        }
                    };
                });       
            services.AddHttpContextAccessor();
            // Singleton Service
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IFileStorageService, CloudinaryStorageService>();
            // My services
            //services.AddScoped<ICollaborator, Collaborator>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IToken, TokenRepository>();                
            services.AddScoped<IMailService, MailService>()
                .Configure<EmailConfiguration>(Configuration.GetSection("AppSettings:EmailSettings"));
            services.AddScoped<IPaymentService, VNPayPaymentService>()
                .Configure<VNPayPaymentConfiguration>(Configuration.GetSection("AppSettings:VNPay"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseForwardedHeaders(new ForwardedHeadersOptions() { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.All });
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AdsMarketSharing API");
                c.RoutePrefix = String.Empty;
            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions() { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.All });
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseCors("AllowAPIRequestIO");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapControllerRoute(
                //        name: "Payment_default",
                //        pattern: "Payment/{controller}/{action}/{id?}",
                //        defaults: new { controller = "VNPayment", action = "Index", id = UrlParameter.Optional });
                endpoints.MapHub<ChatHub>("/hubs/chat");
                endpoints.MapHub<NotifyHub>("/hubs/notify");
                endpoints.MapHub<ReviewHub>("/hubs/review");
            });
        }
    }
}
