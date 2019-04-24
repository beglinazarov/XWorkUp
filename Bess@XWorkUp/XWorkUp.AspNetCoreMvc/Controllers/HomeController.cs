using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using XWorkUp.AspNetCoreMvc.Models;
using XWorkUp.AspNetCoreMvc.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace XWorkUp.AspNetCoreMvc.Controllers
{
	public class HomeController : Controller
	{
		private readonly IPieRepository _pieRepository;
		HtmlEncoder _htmlEncoder;

		public HomeController(HtmlEncoder htmlEncoder, IPieRepository pieRepository)
		{
			//Sanitizing input via HtmlEncoder there are alternative way JavascriptEncoder and UrlEncoder
			_htmlEncoder = htmlEncoder;
			_pieRepository = pieRepository;
		}

		public ViewResult Index()
		{
			var homeViewModel = new HomeViewModel
			{
				PiesOfTheWeek = _pieRepository.PiesOfTheWeek
			};

			return View(homeViewModel);
		}
	}
}
