using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.Core.Entities;

namespace TiendaOnline.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSet para cada entidad personalizada
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuración de ApplicationUser y Value Object Address
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.OwnsOne(user => user.Address, address =>
                {
                    address.Property(a => a.Street).HasMaxLength(100).IsRequired();
                    address.Property(a => a.City).HasMaxLength(50).IsRequired();
                    address.Property(a => a.State).HasMaxLength(50).IsRequired();
                    address.Property(a => a.ZipCode).HasMaxLength(10).IsRequired();
                    address.Property(a => a.Country).HasMaxLength(50).IsRequired();
                });
            });

            // Personalización de las tablas de Identity (opcional)
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");

            // Relación muchos-a-muchos para OrderProduct
            builder.Entity<OrderProduct>()
                .HasKey(op => new { op.OrderId, op.ProductId });

            builder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.OrderId);

            builder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany(p => p.OrderProducts)
                .HasForeignKey(op => op.ProductId);

            // Relación de Cart y CartItems
            builder.Entity<Cart>()
                .HasMany(c => c.Items)
                .WithOne()
                .HasForeignKey(ci => ci.Id);

            builder.Entity<CartItem>()
                .Property(ci => ci.Price)
                .HasColumnType("decimal(18,2)");

            // Configuración de Product
            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // Relación uno-a-muchos de Product a ProductImage
            builder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId);

            builder.Entity<ProductImage>()
                .Property(pi => pi.ImageUrl)
                .IsRequired();
        }
    }
}
