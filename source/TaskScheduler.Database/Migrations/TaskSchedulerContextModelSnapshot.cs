﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskScheduler.Database;

#nullable disable

namespace TaskScheduler.Database.Migrations
{
    [DbContext(typeof(TaskSchedulerContext))]
    partial class TaskSchedulerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Participation", b =>
                {
                    b.Property<int>("ParticipantUserIdUserId")
                        .HasColumnType("int");

                    b.Property<int>("ParticipatedTaskIdTaskId")
                        .HasColumnType("int");

                    b.HasKey("ParticipantUserIdUserId", "ParticipatedTaskIdTaskId");

                    b.HasIndex("ParticipatedTaskIdTaskId");

                    b.ToTable("Participation");
                });

            modelBuilder.Entity("TaskScheduler.Domain.LoginAuth", b =>
                {
                    b.Property<int>("AuthId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AuthId");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.ToTable("LoginAuths");
                });

            modelBuilder.Entity("TaskScheduler.Domain.ScheduleTask", b =>
                {
                    b.Property<int>("TaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TaskId"), 1L, 1);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsRepetitive")
                        .HasColumnType("bit");

                    b.Property<string>("Link")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("OwnerUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("RepetitiveEnd")
                        .HasColumnType("datetime2");

                    b.Property<int>("RepetitivePeriod")
                        .HasColumnType("int");

                    b.Property<DateTime>("TaskEnd")
                        .HasColumnType("datetime2");

                    b.Property<int>("TaskPriority")
                        .HasColumnType("int");

                    b.Property<DateTime>("TaskStart")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TaskId");

                    b.HasIndex("Link")
                        .IsUnique()
                        .HasFilter("[Link] IS NOT NULL");

                    b.HasIndex("OwnerUserId");

                    b.ToTable("ScheduleTasks");
                });

            modelBuilder.Entity("TaskScheduler.Domain.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"), 1L, 1);

                    b.Property<int>("LoginAuthId")
                        .HasColumnType("int");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Participation", b =>
                {
                    b.HasOne("TaskScheduler.Domain.User", "ParticipantUserId")
                        .WithMany()
                        .HasForeignKey("ParticipantUserIdUserId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("TaskScheduler.Domain.ScheduleTask", "ParticipatedTaskId")
                        .WithMany()
                        .HasForeignKey("ParticipatedTaskIdTaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ParticipantUserId");

                    b.Navigation("ParticipatedTaskId");
                });

            modelBuilder.Entity("TaskScheduler.Domain.LoginAuth", b =>
                {
                    b.HasOne("TaskScheduler.Domain.User", "User")
                        .WithOne("LoginAuth")
                        .HasForeignKey("TaskScheduler.Domain.LoginAuth", "AuthId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("TaskScheduler.Domain.ScheduleTask", b =>
                {
                    b.HasOne("TaskScheduler.Domain.User", "Owner")
                        .WithMany("OwnedTasks")
                        .HasForeignKey("OwnerUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("TaskScheduler.Domain.User", b =>
                {
                    b.Navigation("LoginAuth");

                    b.Navigation("OwnedTasks");
                });
#pragma warning restore 612, 618
        }
    }
}
