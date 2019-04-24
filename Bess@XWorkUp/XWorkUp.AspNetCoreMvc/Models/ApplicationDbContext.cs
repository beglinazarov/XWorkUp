using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using XWorkUp.AspNetCoreMvc.Auth;

namespace XWorkUp.AspNetCoreMvc.Models
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
			base.OnModelCreating(builder);
			builder.Entity<Pie>()
				.HasOne(p => p.RecipeInformation)
				.WithOne(i => i.Pie)
				.HasForeignKey<RecipeInformation>(b => b.PieId);
			
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

		public DbSet<Pie> Pies { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set; }
		public DbSet<PieReview> PieReviews { get; set; }
		public DbSet<PieGiftOrder> PieGiftOrders { get; set; }
	}
}
