using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using XWorkUp.AspNetCoreMvc.Models;
using XWorkUp.AspNetCoreMvc.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace XWorkUp.AspNetCoreMvc.Controllers
{
	public class PieController : Controller
    {
		HtmlEncoder _htmlEncoder;
		private readonly IPieRepository _pieRepository;
        private readonly IPieReviewRepository _pieReviewRepository;
        private readonly ICategoryRepository _categoryRepository;

		public PieController(HtmlEncoder htmlEncoder, IPieRepository pieRepository,
							ICategoryRepository categoryRepository, IPieReviewRepository pieReviewRepository)
        {
			_htmlEncoder = htmlEncoder;
            _pieRepository = pieRepository;
			_pieReviewRepository = pieReviewRepository;
            _categoryRepository = categoryRepository;
        }

        //public ViewResult List()
        //{
        //    PiesListViewModel piesListViewModel = new PiesListViewModel();
        //    piesListViewModel.Pies = _pieRepository.Pies;

        //    piesListViewModel.CurrentCategory = "Cheese cakes";

        //    return View(piesListViewModel);
        //}

        public ViewResult List(string category)
        {
            IEnumerable<Pie> pies;
            string currentCategory = string.Empty;

            if (string.IsNullOrEmpty(category))
            {
                pies = _pieRepository.Pies.OrderBy(p => p.PieId);
                currentCategory = "All pies";
            }
            else
            {
                pies = _pieRepository.Pies.Where(p => p.Category.CategoryName == category)
                   .OrderBy(p => p.PieId);
                currentCategory = _categoryRepository.Categories.FirstOrDefault(c => c.CategoryName == category).CategoryName;
            }

            return View(new PiesListViewModel
            {
                Pies = pies,
                CurrentCategory = currentCategory
            });
        }

		[Route("[controller]/Details/{id}")]
		public IActionResult Details(int id)
		{
			var pie = _pieRepository.GetPieById(id);
			if (pie == null)
				return NotFound();

			if (pie.PieReviews == null)
				pie.PieReviews = new List<PieReview>() { new PieReview() { Pie = pie, PieReviewId = 1, Review = "tasty!" } };

			return View(new PieDetailViewModel() { Pie = pie });
		}

		[Route("[controller]/Details/{id}")]
		[HttpPost]
		public IActionResult Details(int id, string review)
		{
			var pie = _pieRepository.GetPieById(id);
			if (pie == null)
				return NotFound();

			string encodedReview = _htmlEncoder.Encode(review);

			_pieReviewRepository.AddPieReview(new PieReview() { Pie = pie, Review = encodedReview });

			return View(new PieDetailViewModel() { Pie = pie });
		}

	}
}
