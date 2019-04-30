using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Caching.Memory;
using XWorkUp.AspNetCoreMvc.Filters;
using XWorkUp.AspNetCoreMvc.Utility;
using XWorkUp.AspNetCoreMvc.Models;
using XWorkUp.AspNetCoreMvc.ViewModels;
using System.Linq;
using System.Collections.Generic;

namespace XWorkUp.AspNetCoreMvc.Controllers
{
	//[RequireHeader]
	//[TimerAction]
	//[ServiceFilter(typeof(TimerAction))]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private IMemoryCache _memoryCache;
		HtmlEncoder _htmlEncoder;
		private readonly IStringLocalizer<HomeController> _stringLocalizer;
		private readonly IPieRepository _pieRepository;

		public HomeController(
					ILogger<HomeController> logger,
					IMemoryCache memoryCache,
					HtmlEncoder htmlEncoder,
					IStringLocalizer<HomeController> stringLocalizer,
					IPieRepository pieRepository
			)
		{
			_logger = logger;
			_memoryCache = memoryCache;
			_htmlEncoder = htmlEncoder;
			_stringLocalizer = stringLocalizer;
			_pieRepository = pieRepository;
		}

		//[ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client)]
		//[ResponseCache(Duration = 30, VaryByHeader = "User-Agent")]
		//[ResponseCache(CacheProfileName = "None")]
		[ResponseCache(Duration = 30)]
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

			//caching change for IMemoryCache
			List<Pie> piesOfTheWeekCached = null;

			//if (!_memoryCache.TryGetValue(CacheEntryConstants.PiesOfTheWeek, out piesOfTheWeekCached))
			//{
			//	piesOfTheWeekCached = _pieRepository.PiesOfTheWeek.ToList();
			//	var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));
			//	cacheEntryOptions.RegisterPostEvictionCallback(FillCacheAgain, this);

			//	_memoryCache.Set(CacheEntryConstants.PiesOfTheWeek, piesOfTheWeekCached, cacheEntryOptions);
			//}

			piesOfTheWeekCached = _memoryCache.GetOrCreate(CacheEntryConstants.PiesOfTheWeek, entry =>
			{
				entry.SlidingExpiration = TimeSpan.FromSeconds(10);
				entry.Priority = CacheItemPriority.High;
				return _pieRepository.PiesOfTheWeek.ToList();
			});

			var homeViewModel = new HomeViewModel
			{
				PiesOfTheWeek = piesOfTheWeekCached
			};

			return View(homeViewModel);
		}

		private void FillCacheAgain(object key, object value, EvictionReason reason, object state)
		{
			_logger.LogInformation(LogEventIds.LoadHomepage, "Cache was cleared: reason " + reason.ToString());
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
