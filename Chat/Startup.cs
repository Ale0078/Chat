using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using Chat.Server.Hubs;
using Chat.Server.Authentication;
using Chat.Server.Services;
using Chat.Server.Services.Interfaces;
using Chat.Server.AutoMapperProfiles;
using Chat.Entities;
using Chat.Entities.Contexts;

namespace Chat.Server
{
    public class Startup
    {
        private const int MIN_PASSWORD_LENGHT = 6;
        private const string MAP_HUB = "/chat";
        private const string MAP_HUB_REGISTER = "/chat_register";
        private const string DEFAULT_CONNECTION_STRING_KEY = "DefaultConnection";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString(DEFAULT_CONNECTION_STRING_KEY));
            });

            services.AddIdentity<User, IdentityRole>(options => 
            {
                options.Password.RequiredLength = MIN_PASSWORD_LENGHT;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<ApplicationContext>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SecurityTokenValidators.Clear();
                options.SecurityTokenValidators.Add(new NameTokenValidator());
            });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireClaim(ClaimTypes.Name)
                    .Build();
            });

            services.AddSignalR(options => 
            {
                options.MaximumReceiveMessageSize = long.MaxValue;
            });


            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IGroupService, GroupService>();

            services.AddAutoMapper(typeof(UserProfile), typeof(MessageProfile), typeof(ChatProfile), typeof(BlockedProfile), typeof(GroupProfile));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>(MAP_HUB);
                endpoints.MapHub<ChatRegistrationHub>(MAP_HUB_REGISTER);
            });
        }
    }
}
