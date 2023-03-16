using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopApp.Entites;

namespace ShopApp.DataAccess.Concrete.EfCore
{
	public class ShopContext : IdentityDbContext<AppUser, AppRole, int>
	{

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("server=CODER;database=DbShopp;integrated security=true");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			//fluent apı
			modelBuilder.Entity<ProductCategory>()
				.HasKey(c => new { c.CategoryId, c.ProductId });
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Order>()
					 .Property(p => p.UserId)
					 .HasColumnName("UserId");

		}


		public DbSet<Product>? Products { get; set; }
		public DbSet<Cart> Carts { get; set; }
		public DbSet<Category>? Categories { get; set; }
		public DbSet<Order>? Orders { get; set; }



	}
}
