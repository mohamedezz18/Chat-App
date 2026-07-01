using DatingApp.Core.Domain.IdentityEntities;
using DatingApp.Core.Domain.RepositoryContracts;
using DatingApp.Core.ServiceContracts;
using DatingApp.Core.Services;
using DatingApp.Core.Settings;
using DatingApp.Infrastructure.DbContext;
using DatingApp.Infrastructure.Repositories;
using DatingApp.UI.Filters;
using DatingApp.UI.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace DatingApp.UI
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];

            services.AddControllers();

            //Registering the Repositories in the DI container
            services.AddScoped<IMemberRepository, MemberRepository>();

            //Registering the Services in the DI container
            services.AddScoped<IMembersService, MembersService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<LogUserActivity>();
            services.Configure<Jwt>(configuration.GetSection("JWT"));
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.AddScoped<ILikesRepository, LikesRepository>();
            services.AddScoped<ILikesService, LikesService>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddSignalR();
            services.AddSingleton<PresenceTracker>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 3; //Eg: AB12AB (unique characters are A,B,1,2)
                options.User.RequireUniqueEmail = true;
            })
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders();
           // .AddUserStore<UserStore<AppUser, IdentityRole<Guid>, ApplicationDbContext, Guid>>()
            // .AddRoleStore<RoleStore<IdentityRole<Guid>, ApplicationDbContext, Guid>>();


            //Adding CORS policy to allow requests from "4220"
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policyBuilder =>
                {
                    if (allowedOrigins.Length > 0)
                    {
                        policyBuilder.WithOrigins(allowedOrigins);
                    }

                    policyBuilder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.AddAuthorization(option =>
            {
                option.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                 option.AddPolicy("ModeratePhotoRole",
                     policy => policy.RequireRole("Admin", "Moderator"));
            });
    

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(o =>
             {
                //o.RequireHttpsMetadata = false;
                //o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"])),
                    ClockSkew = TimeSpan.Zero
                };

                 o.Events = new JwtBearerEvents
                 {
                     OnAuthenticationFailed = context =>
                     {
                         Console.WriteLine(context.Exception.Message);
                         return Task.CompletedTask;
                     },
                     OnMessageReceived = context =>
                     {
                         var accessToken = context.Request.Query["access_token"];
                         // If the request is for our hub...
                         var path = context.HttpContext.Request.Path;
                         if (!string.IsNullOrEmpty(accessToken) &&
                             path.StartsWithSegments("/hub"))
                         {
                             // Read the token out of the query string
                             context.Token = accessToken;
                         }
                         return Task.CompletedTask;
                     }
                 };
            });

            return services;
        }
    }
}
