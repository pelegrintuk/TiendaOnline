using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.Core.Entities;

namespace TiendaOnline.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

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
        }
    }
}
