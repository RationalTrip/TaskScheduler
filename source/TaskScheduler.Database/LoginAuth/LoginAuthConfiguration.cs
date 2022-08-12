using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskScheduler.Domain;

namespace TaskScheduler.Database
{
    class LoginAuthConfiguration : IEntityTypeConfiguration<LoginAuth>
    {
        public void Configure(EntityTypeBuilder<LoginAuth> builder)
        {
            builder.HasKey(auth => auth.AuthId);
            builder.Property(auth => auth.AuthId).ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(auth => auth.Login).IsRequired();
            builder.HasIndex(auth => auth.Login).IsUnique();

            builder.Property(auth => auth.Password).IsRequired();

            builder.Property(auth => auth.Salt).IsRequired();

            builder.HasOne(auth => auth.User)
                .WithOne(user => user.LoginAuth)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
