using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wize.settings.odata.Config
{
    public static class Authentication
    {
        public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration)
        {
            JwtModel jwt = new JwtModel();
            jwt.ValidAudience = configuration.GetValue<string>("JwtAuthentication_ValidAudience");
            jwt.ValidIssuer = configuration.GetValue<string>("JwtAuthentication_ValidIssuer");

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;
                options.RequireHttpsMetadata = false;
                options.Authority = jwt.ValidIssuer;
                options.Audience = jwt.ValidAudience;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("read:setting", policy => policy.Requirements.Add(new HasPermissionsRequirement("read:setting", jwt.ValidIssuer)));
                options.AddPolicy("add:setting", policy => policy.Requirements.Add(new HasPermissionsRequirement("add:setting", jwt.ValidIssuer)));
                options.AddPolicy("list:setting", policy => policy.Requirements.Add(new HasPermissionsRequirement("list:setting", jwt.ValidIssuer)));
                options.AddPolicy("update:setting", policy => policy.Requirements.Add(new HasPermissionsRequirement("update:setting", jwt.ValidIssuer)));
                options.AddPolicy("delete:setting", policy => policy.Requirements.Add(new HasPermissionsRequirement("delete:setting", jwt.ValidIssuer)));
            });
            services.AddSingleton<IAuthorizationHandler, HasPermissionsHandler>();

            return services;
        }

        public static IApplicationBuilder UseJwt(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}
