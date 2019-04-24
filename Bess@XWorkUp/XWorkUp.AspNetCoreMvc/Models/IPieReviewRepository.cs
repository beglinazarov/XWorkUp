using System.Collections.Generic;

namespace XWorkUp.AspNetCoreMvc.Models
{
	public interface IPieReviewRepository
    {
        void AddPieReview(PieReview pieReview);
        IEnumerable<PieReview> GetReviewsForPie(int pieId);
    }
}
