using System.Collections.Generic;
using System.Linq;

namespace XWorkUp.AspNetCoreMvc.Models
{
	public class PieReviewRepository : IPieReviewRepository
    {
        private readonly ApplicationDbContext _appDbContext;

        public PieReviewRepository(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void AddPieReview(PieReview pieReview)
        {
            _appDbContext.PieReviews.Add(pieReview);
            _appDbContext.SaveChanges();
        }

        public IEnumerable<PieReview> GetReviewsForPie(int pieId)
        {
            return _appDbContext.PieReviews.Where(p => p.Pie.PieId == pieId);
        }
    }
}
