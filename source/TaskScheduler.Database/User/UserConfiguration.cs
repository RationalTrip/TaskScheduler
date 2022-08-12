using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using TaskScheduler.Domain;

namespace TaskScheduler.Database
{
    class UserConfiguration : IEntityTypeConfiguration<User>
    {
        void IEntityTypeConfiguration<User>.Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(user => user.UserId);
            builder.Property(user => user.UserId)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.HasOne(user => user.LoginAuth)
                .WithOne(auth => auth.User)
                .HasForeignKey<LoginAuth>(user => user.AuthId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(user => user.OwnedTasks)
                .WithOne(task => task.Owner);

            builder.HasMany(user => user.ParticipatedTasks)
                .WithMany(task => task.Participants)
                .UsingEntity<Dictionary<string, object>>(
                    "Participation",
                    taskToUser => taskToUser.HasOne<ScheduleTask>("ParticipatedTaskId").WithMany().OnDelete(DeleteBehavior.Cascade),
                    taskToUser => taskToUser.HasOne<User>("ParticipantUserId").WithMany().OnDelete(DeleteBehavior.ClientCascade)
                );
        }
    }
}
