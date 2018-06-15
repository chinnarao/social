using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using account.utility;

namespace account
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
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy(JwtBearerDefaults.AuthenticationScheme, new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                    .RequireAuthenticatedUser().Build());
            });

            var tokenValidationParameters = new TokenValidationParameters()
            {
                // When receiving a token, check that we've signed it.
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = Jwt.Key,
                ValidateAudience = true,
                ValidAudience = Configuration["JWT_TOKEN:Audience"],
                ValidateIssuer = true,
                ValidIssuer = Configuration["JWT_TOKEN:Issuer"],
            };
            // Enable the use of an  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]   attribute on methods and classes to protect.
            services.AddAuthentication(options => { options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; })
            .AddJwtBearer(jwt =>
            {
                jwt.RequireHttpsMetadata = false; jwt.SaveToken = true;
                jwt.TokenValidationParameters = tokenValidationParameters;
                jwt.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = c =>
                    {
                        c.NoResult(); c.Response.StatusCode = 500; c.Response.ContentType = "text/plain";
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = c =>
                    {
                        //Console.WriteLine("OnTokenValidated: " + c.SecurityToken); todo : logging
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddDbContext<AppUsersDbContext>(options => options.UseSqlServer(Configuration["DB_CONN"]));

            services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<AppUsersDbContext>().AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 1; options.Password.RequireNonAlphanumeric = false; options.Password.RequireLowercase = false; options.Password.RequireUppercase = false;
            });
            services.AddCors();

            //services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            }); ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(c => c.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
