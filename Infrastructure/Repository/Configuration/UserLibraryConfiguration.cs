using Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repository.Configuration;

public class UserLibraryConfiguration : IEntityTypeConfiguration<UserLibrary>
{
    public void Configure(EntityTypeBuilder<UserLibrary> builder)
    {
        builder.ToTable("UsersLibrary");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnType("INT").ValueGeneratedNever().UseMySqlIdentityColumn();
        builder.Property(x=>x.UserId).HasColumnType("INT").IsRequired();
        builder.Property(x=>x.GameId).HasColumnType("INT").IsRequired();
        builder.Property(x=> x.CreatedAt).HasColumnType("DATETIME").IsRequired();
        
        builder.HasOne(u=>u.User).WithMany().HasPrincipalKey(u=>u.Id);
        
        builder.HasOne(u=>u.Game).WithMany().HasPrincipalKey(u=>u.Id);
        
    }
}