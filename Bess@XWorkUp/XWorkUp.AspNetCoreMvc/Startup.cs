using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XWorkUp.AspNetCoreMvc.Models;
using XWorkUp.AspNetCoreMvc.Services;
using XWorkUp.AspNetCoreMvc.Auth;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using XWorkUp.AspNetCoreMvc.Filters;

namespace XWorkUp.AspNetCoreMvc
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
			//.AddJsonFile($"secrets.{env.EnvironmentName}.json", optional: true);

			builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			//services.Configure<CookiePolicyOptions>(options =>
			//{
			//	// This lambda determines whether user consent for non-essential cookies is needed for a given request.
			//	options.CheckConsentNeeded = context => true;
			//	options.MinimumSameSitePolicy = SameSiteMode.None;
			//});

			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(
					Configuration.GetConnectionString("DefaultConnection")));

			//services.AddScoped<SignInManager<ApplicationUser>, SignInManager<ApplicationUser>>();

			services.ConfigureApplicationCookie(options =>
			{
				// Cookie settings
				options.Cookie.HttpOnly = true;
				options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

				options.LoginPath = new PathString("/Account/Login");
				options.AccessDeniedPath = "/Account/AccessDenied";
				options.SlidingExpiration = true;
			});

			services.AddTransient<IPieRepository, PieRepository>();
			services.AddTransient<ICategoryRepository, CategoryRepository>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddScoped(sp => ShoppingCart.GetCart(sp));
			services.AddTransient<IOrderRepository, OrderRepository>();
			services.AddTransient<IPieReviewRepository, PieReviewRepository>();
			//Filters
			services.AddScoped<TimerAction>();
			//specify options for the anti forgery here
			services.AddAntiforgery();

			//anti forgery as global filter
			//services.AddMvc(options =>
			//	options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

			services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

			services.AddMvc()
				.AddViewLocalization(
					LanguageViewLocationExpanderFormat.Suffix,
					opts => { opts.ResourcesPath = "Resources"; })
				.AddDataAnnotationsLocalization()
				.AddRazorPagesOptions(options =>
					{
						options.Conventions.AuthorizeFolder("/Account/Manage");
						options.Conventions.AuthorizePage("/Account/Logout");
					})
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			services.Configure<RequestLocalizationOptions>(
				options =>
				{
					var supportedCultures = new List<CultureInfo>
					{
						new CultureInfo("fr"),
						new CultureInfo("fr-FR"),
						new CultureInfo("nl"),
						new CultureInfo("nl-BE"),
						new CultureInfo("en-US")
					};

					options.DefaultRequestCulture = new RequestCulture("en-US");
					options.SupportedCultures = supportedCultures;
					options.SupportedUICultures = supportedCultures;
				});

			services.AddIdentityCore<ApplicationUser>(options =>
			{
				options.Password.RequiredLength = 8;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequireUppercase = true;
				options.User.RequireUniqueEmail = true;
				options.SignIn.RequireConfirmedEmail = false;
				//options.SignIn.RequireConfirmedPhoneNumber = true;

			})
				.AddRoles<IdentityRole>()
				//	.AddUserStore<UserStore>()
				.AddSignInManager<SignInManager<ApplicationUser>>()
				.AddDefaultUI(UIFramework.Bootstrap4)
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			services.AddAuthentication(o =>
			{
				o.DefaultScheme = IdentityConstants.ApplicationScheme;
				o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
			})
			.AddGoogle(googleOptions =>
			{
				googleOptions.ClientId = "419998585765-18s94828lr7catq8c9nb5dmhd96pgebq.apps.googleusercontent.com";
				googleOptions.ClientSecret = "X_WqRQ4vlkQd4XCR6ZAOP4Dc";
			})
			.AddIdentityCookies(o => { });

			services.AddAuthorization(options =>
			{
				options.AddPolicy("DeletePie", policy => policy.RequireClaim("Delete Pie"));
				options.AddPolicy("AddPie", policy => policy.RequireClaim("Add Pie"));
				options.AddPolicy("MinimumOrderAge", policy => policy.Requirements.Add(new MinimumOrderAgeRequirement(18)));
			});
			services.AddMemoryCache();
			services.AddSession();
			// Add application services.
			services.Configure<AuthMessageSenderOptions>(Configuration.GetSection("AuthMessageSenderOptions"));
			services.AddSingleton<IEmailSender, AuthMessageSender>();
			services.AddTransient<ISmsSender, AuthMessageSender>();

			// requires
			// using Microsoft.AspNetCore.Identity.UI.Services;
			// using WebPWrecover.Services;
			//services.AddTransient<IEmailSender, EmailSender>();
			//services.Configure<AuthMessageSenderOptions>(Configuration.GetSection("AuthMessageSenderOptions"));

			// old approach
			//services.AddIdentityCore<ApplicationUser>()
			//	//new code
			//	.AddRoles<IdentityRole>()
			//	.AddDefaultUI(UIFramework.Bootstrap4)
			//	.AddEntityFrameworkStores<ApplicationDbContext>();


			//services.ConfigureApplicationCookie(options =>
			//{
			//	// Cookie settings
			//	options.Cookie.HttpOnly = true;
			//	options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

			//	options.LoginPath = "/Account/Login";
			//	options.AccessDeniedPath = "/Identity/Account/AccessDenied";
			//	options.SlidingExpiration = true;
			//});


		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
			IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
		{
			// Diagnostics
			// app.UseWelcomePage();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseStatusCodePages();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/AppException");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseCookiePolicy();
			app.UseDeveloperExceptionPage();
			app.UseStatusCodePages();
			app.UseStaticFiles();
			app.UseSession();
			app.UseAuthentication();

			// Logging
			loggerFactory.AddConsole(LogLevel.Debug);
			loggerFactory.AddDebug(LogLevel.Debug);

			//loggerFactory.AddDebug((c, l) => c.Contains("HomeController") && l > LogLevel.Trace);
			//loggerFactory
			//    .WithFilter(new FilterLoggerSettings
			//    {
			//        {"Microsoft", LogLevel.Warning},
			//        {"System", LogLevel.Warning},
			//        {"HomeController", LogLevel.Debug}
			//    }).AddDebug();

			//Serilog
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.RollingFile(Path.Combine(env.ContentRootPath, "XWorkUpsLogs-{Date}.txt"))
				.CreateLogger();

			loggerFactory.AddSerilog();

			app.UseRequestLocalization(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value);
			
			app.UseMvc(routes =>
			{
				//areas
				routes.MapRoute(
					name: "areas",
					template: "{area:exists}/{controller=Home}/{action=Index}");

				routes.MapRoute(
				  name: "categoryfilter",
				  template: "Pie/{action}/{category?}",
				  defaults: new { Controller = "Pie", action = "List" });

				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});

			DbInitializer.Seed(serviceProvider.GetRequiredService<ApplicationDbContext>());
		}
	}
}
