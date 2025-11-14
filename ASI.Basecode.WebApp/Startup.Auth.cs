using System;
using ASI.Basecode.WebApp.Authentication;
using ASI.Basecode.WebApp.Extensions.Configuration;
using ASI.Basecode.Resources.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace ASI.Basecode.WebApp
{
    /// <summary>
    /// Partial class for configuring authentication and authorization services.
    /// Handles JWT bearer token and cookie-based authentication setup.
    /// </summary>
    internal partial class StartupConfigurer
    {
        private readonly SymmetricSecurityKey _signingKey;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly TokenProviderOptions _tokenProviderOptions;

        /// <summary>
        /// Configures authentication and authorization services for the application.
        /// Sets up JWT bearer token authentication and cookie-based authentication with sliding expiration.
        /// </summary>
        private void ConfigureAuthorization()
        {
            var token = Configuration.GetTokenAuthentication();
            var tokenProviderOptionsFactory = this._services.BuildServiceProvider().GetService<TokenProviderOptionsFactory>();
            var tokenValidationParametersFactory = this._services.BuildServiceProvider().GetService<TokenValidationParametersFactory>();
            var tokenValidationParameters = tokenValidationParametersFactory.Create();

            // ============================================
            // AUTHENTICATION CONFIGURATION
            // ============================================
            this._services.AddAuthentication(Const.AuthenticationScheme)
            // JWT Bearer Token Authentication
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;
            })
            // Cookie-based Authentication with Sliding Expiration
            .AddCookie(Const.AuthenticationScheme, options =>
            {
                options.Cookie = new CookieBuilder()
                {
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax,
                    SecurePolicy = CookieSecurePolicy.SameAsRequest,
                    Name = $"{this._environment.ApplicationName}_{token.CookieName}"
                };
                options.LoginPath = new PathString("/Account/Login");
                options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                options.ReturnUrlParameter = "ReturnUrl";
                // Cookie expiration and sliding expiration prevent automatic logouts
                options.ExpireTimeSpan = TimeSpan.FromMinutes(token.ExpirationMinutes);
                options.SlidingExpiration = true;
                options.TicketDataFormat = new CustomJwtDataFormat(SecurityAlgorithms.HmacSha256, _tokenValidationParameters, Configuration, tokenProviderOptionsFactory);
            });

            // ============================================
            // AUTHORIZATION POLICIES
            // ============================================
            this._services.AddAuthorization(options =>
            {
                // Default policy requiring authenticated users for all actions
                options.AddPolicy("RequireAuthenticatedUser", policy =>
                {
                    policy.RequireAuthenticatedUser();
                });
            });

            // ============================================
            // MVC AUTHORIZATION FILTERS
            // ============================================
            // Apply default authorization policy to all MVC actions
            this._services.AddMvc(options =>
            {
                options.Filters.Add(new AuthorizeFilter("RequireAuthenticatedUser"));
            });
        }
    }
}
