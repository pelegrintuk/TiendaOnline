using Microsoft.EntityFrameworkCore;
using TiendaOnline.Core.Entities;

namespace TiendaOnline.DAL
{
    public class TiendaContext : DbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }

        // DbSet para cada entidad personalizada
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación muchos-a-muchos para OrderProduct
            modelBuilder.Entity<OrderProduct>()
                .HasKey(op => new { op.OrderId, op.ProductId });

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.OrderId);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany(p => p.OrderProducts)
                .HasForeignKey(op => op.ProductId);

            // Relación de Cart y CartItems
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.Items)
                .WithOne()
                .HasForeignKey(ci => ci.Id);

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.Price)
                .HasColumnType("decimal(18,2)");

            // Configuración de Product
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // Relación uno-a-muchos de Product a ProductImage
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId);

            modelBuilder.Entity<ProductImage>()
                .Property(pi => pi.ImageUrl)
                .IsRequired();
        }
    }
}
