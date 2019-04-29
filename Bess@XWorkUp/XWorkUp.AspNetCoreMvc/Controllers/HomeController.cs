using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Encodings.Web;
using XWorkUp.AspNetCoreMvc.Models;
using XWorkUp.AspNetCoreMvc.ViewModels;
using XWorkUp.AspNetCoreMvc.Filters;

namespace XWorkUp.AspNetCoreMvc.Controllers
{
	//[RequireHeader]
	//[TimerAction]
	[ServiceFilter(typeof(TimerAction))]
	public class HomeController : Controller
	{
		private readonly IPieRepository _pieRepository;
		private readonly IStringLocalizer<HomeController> _stringLocalizer;
		private readonly ILogger<HomeController> _logger;
		HtmlEncoder _htmlEncoder;

		public HomeController(IPieRepository pieRepository,
					IStringLocalizer<HomeController> stringLocalizer,
					ILogger<HomeController> logger,
					HtmlEncoder htmlEncoder)
		{
			_pieRepository = pieRepository;
			_stringLocalizer = stringLocalizer;
			_logger = logger;
			_htmlEncoder = htmlEncoder;
		}

		public ViewResult Index()
		{
			// Logging
			//_logger.LogInformation(LogEventIds.LoadHomepage,"Loading home page");

			// Serilog
			_logger.LogDebug("Loading home page");

			//ViewData["PageTitle"] = _stringLocalizer["Welcome to Bethany's Pie Shop"];
			ViewBag.PageTitle = _stringLocalizer["PageTitle"];
			ViewData["PiesOfTheWeek"] = _stringLocalizer["PiesOfTheWeek"];
			ViewData["NonExistingKey"] = _stringLocalizer["NonExistingKey"];


			var homeViewModel = new HomeViewModel
			{
				PiesOfTheWeek = _pieRepository.PiesOfTheWeek
			};

			return View(homeViewModel);
		}

		public IActionResult SetLanguage(string culture, string returnUrl)
		{
			Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
				CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
				new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
			);

			return LocalRedirect(returnUrl);
		}
	}
}
