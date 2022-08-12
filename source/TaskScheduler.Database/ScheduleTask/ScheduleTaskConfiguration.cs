using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using TaskScheduler.Domain;

namespace TaskScheduler.Database
{
    class ScheduleTaskConfiguration : IEntityTypeConfiguration<ScheduleTask>
    {
        void IEntityTypeConfiguration<ScheduleTask>.Configure(EntityTypeBuilder<ScheduleTask> builder)
        {
            builder.HasKey(task => task.TaskId);
            builder.Property(task => task.TaskId)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.HasIndex(task => task.Link)
                .IsUnique();

            builder.Property(task => task.Title)
                .IsRequired();

            builder.Property(task => task.TaskStart)
                .IsRequired();

            builder.Property(task => task.TaskEnd)
                .IsRequired();

            builder.Property(task => task.IsRepetitive)
                .IsRequired();

            builder.Ignore(task => task.RepetitiveStart);

            builder.Property(task => task.TaskPriority)
                .IsRequired();

            builder.HasOne(task => task.Owner)
                .WithMany(user => user.OwnedTasks)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(task => task.Participants)
                .WithMany(user => user.ParticipatedTasks)
                .UsingEntity<Dictionary<string, object>>(
                    "Participation",
                    taskToUser => taskToUser.HasOne<User>("ParticipantUserId").WithMany().OnDelete(DeleteBehavior.ClientCascade),
                    taskToUser => taskToUser.HasOne<ScheduleTask>("ParticipatedTaskId").WithMany().OnDelete(DeleteBehavior.Cascade)
                );
        }
    }
}
