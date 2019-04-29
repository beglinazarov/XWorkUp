using XWorkUp.AspNetCoreMvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace XWorkUp.AspNetCoreMvc.Components
{
	public class CategoryMenu : ViewComponent
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryMenu(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

		public IViewComponentResult Invoke()
		{
			var categories = _categoryRepository.Categories.OrderBy(c => c.CategoryName);

			return View(categories);
		}
	}
}
