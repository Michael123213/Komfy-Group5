using ASI.Basecode.Data;
using ASI.Basecode.Resources.Constants;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.WebApp.Authentication;
using ASI.Basecode.WebApp.Extensions.Configuration;
using ASI.Basecode.WebApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Text;
// --- ADDED USINGS BELOW ---
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services;
// --- END ADDED USINGS ---

namespace ASI.Basecode.WebApp
{
    /// <summary>
    /// For configuring services on application startup.
    /// </summary>
    /// <remarks>
    /// <para>Method call sequence for instances of this class:</para>
    /// <para>1. constructor</para>
    /// <para>2. <see cref="ConfigureServices(IServiceCollection)"/></para>
    /// <para>3. (create <see cref="IApplicationBuilder"/> instance)</para>
    /// <para>4. <see cref="ConfigureApp(IApplicationBuilder, IWebHostEnvironment)"/></para>
    /// </remarks>
    internal partial class StartupConfigurer
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        private IConfiguration Configuration { get; }

        private IApplicationBuilder _app;

        private IWebHostEnvironment _environment;

        private IServiceCollection _services;

        /// <summary>
        /// Initialize new <see cref="StartupConfigurer"/> instance using <paramref name="configuration"/>
        /// </summary>
        /// <param name="configuration"></param>
        public StartupConfigurer(IConfiguration configuration)
        {
            this.Configuration = configuration;

            PathManager.Setup(this.Configuration.GetSetupRootDirectoryPath());

            var token = this.Configuration.GetTokenAuthentication();
            this._signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.SecretKey));
            this._tokenProviderOptions = TokenProviderOptionsFactory.Create(token, this._signingKey);
            this._tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = this._signingKey,
                ValidateIssuer = true,
                ValidIssuer = Const.Issuer,
                ValidateAudience = true,
                ValidAudience = token.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            PasswordManager.SetUp(this.Configuration.GetSection("TokenAuthentication"));
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// Configures application services including dependency injection, database context, 
        /// authentication, authorization, and middleware components.
        /// </summary>
        /// <param name="services">Service collection to configure</param>
        public void ConfigureServices(IServiceCollection services)
        {
            this._services = services;

            // ============================================
            // CACHING SERVICES
            // ============================================
            services.AddMemoryCache();
            services.AddResponseCaching();

            // ============================================
            // DATABASE CONFIGURATION
            // ============================================
            services.AddDbContext<AsiBasecodeDBContext>(options =>
            {
                options.UseSqlServer(
                  Configuration.GetConnectionString("DefaultConnection"),
                  sqlServerOptions => sqlServerOptions.CommandTimeout(120));
            });

            // ============================================
            // MVC AND RAZOR PAGES
            // ============================================
            services.AddControllersWithViews();
            services.AddRazorPages().AddRazorRuntimeCompilation();

            // ============================================
            // CONFIGURATION BINDING
            // ============================================
            services.Configure<TokenAuthentication>(Configuration.GetSection("TokenAuthentication"));

            // ============================================
            // SESSION CONFIGURATION
            // ============================================
            services.AddSession(options =>
            {
                options.Cookie.Name = Const.Issuer;
            });

            // ============================================
            // DEPENDENCY INJECTION SETUP
            // ============================================
            this.ConfigureAutoMapper();
            this.ConfigureOtherServices();
            this.ConfigureAuthorization();

            // ============================================
            // FILE UPLOAD CONFIGURATION
            // ============================================
            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = 1024 * 1024 * 100; // 100MB limit
            });

            // ============================================
            // FILE PROVIDER CONFIGURATION
            // ============================================
            services.AddSingleton<IFileProvider>(
              new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));
        }

        /// <summary>
        /// Configures the HTTP request pipeline with middleware components.
        /// Middleware order is critical and must follow the sequence defined here.
        /// </summary>
        /// <param name="app">Application builder for configuring middleware pipeline</param>
        /// <param name="env">Hosting environment information</param>
        public void ConfigureApp(IApplicationBuilder app, IWebHostEnvironment env)
        {
            this._app = app;
            this._environment = env;

            // ============================================
            // SECURITY MIDDLEWARE
            // ============================================
            if (!this._environment.IsDevelopment())
            {
                this._app.UseHsts();
            }

            // ============================================
            // LOGGING CONFIGURATION
            // ============================================
            this.ConfigureLogger();

            // ============================================
            // AUTHENTICATION MIDDLEWARE
            // ============================================
            this._app.UseTokenProvider(_tokenProviderOptions);

            // ============================================
            // HTTP AND STATIC FILES
            // ============================================
            this._app.UseHttpsRedirection();
            this._app.UseStaticFiles();
            this._app.UseResponseCaching();

            // ============================================
            // LOCALIZATION
            // ============================================
            var options = this._app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            this._app.UseRequestLocalization(options.Value);

            // ============================================
            // SESSION AND ROUTING
            // ============================================
            this._app.UseSession();
            this._app.UseRouting();

            // ============================================
            // AUTHENTICATION AND AUTHORIZATION
            // ============================================
            this._app.UseAuthentication();
            this._app.UseAuthorization();
        }
    }
}
