using Microsoft.EntityFrameworkCore;

namespace LinqExpressionParser.Tests.AspNetCore.Models
{
    public class TestModelDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<OrderSet> OrderSets { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(builder =>
            {
                builder.HasOne(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.RoleId).IsRequired();
                builder.HasOne(u => u.Membership).WithMany(m => m.Users).HasForeignKey(u => u.MembershipId).IsRequired();
                builder.HasMany(u => u.OrderSets).WithOne(o => o.User).HasForeignKey(o => o.UserId).IsRequired();
                builder.HasMany(u => u.Reviews).WithOne(r => r.User).HasForeignKey(r => r.UserId).IsRequired();
                builder.HasMany(u => u.Points).WithOne(p => p.User).HasForeignKey(p => p.UserId).IsRequired();
            });

            modelBuilder.Entity<Item>(builder =>
            {
                builder.HasOne(i => i.Category).WithMany(c => c.Items).HasForeignKey(i => i.CategoryId).IsRequired();
                builder.HasMany(i => i.Reviews).WithOne(r => r.Item).HasForeignKey(r => r.ItemId).IsRequired();
            });

            modelBuilder.Entity<Category>(builder =>
            {
                builder.HasOne(c => c.ParentCategory).WithMany(c => c.ChildCategories).HasForeignKey(c => c.ParentCategoryId);
            });

            modelBuilder.Entity<OrderSet>(builder =>
            {
                builder.HasMany(o => o.Orders).WithOne(o => o.OrderSet).HasForeignKey(o => o.OrderSetId);
            });
        }
    }
}
