using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using FluentValidation.AspNetCore;
using TaskScheduler.Model;
using TaskScheduler.Application;
using TaskScheduler.Database;
using TaskScheduler.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Diagnostics;

namespace TaskScheduler.Web
{
    static class StartupExtensions
    {
        public static void AddFluentValidation(this IServiceCollection services)
        {
            services.AddFluentValidationClientsideAdapters();

            services.AddScoped<IValidator<LoginAuthRegisterModel>, LoginAuthRegisterModelValidator>();
            services.AddScoped<IValidator<LoginAuthSignInModel>, LoginAuthSignInModelValidator>();

            services.AddScoped<IValidator<ScheduleTaskCreateModel>, ScheduleTaskCreateModelValidator>();
        }
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IScheduleTaskLinkGenerator, ScheduleTaskLinkGenerator>();
            services.AddTransient<IScheduleTaskToIndividualTaskConvertor, ScheduleTaskToIndividualTaskConvertor>();

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ILoginAuthService, LoginAuthService>();
            services.AddTransient<IScheduleTaskService, ScheduleTaskService>();
        }
        public static void AddRepository(this IServiceCollection services)
        {
            services.AddTransient<IPasswordHasher, PasswordHasher>();
            services.AddTransient<ISaltGenerator, SaltGenerator>();

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ILoginAuthRepository, LoginAuthRepository>();
            services.AddTransient<IScheduleTaskRepository, ScheduleTaskRepository>();
        }
        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString(nameof(TaskSchedulerContext));
            services.AddDbContext<TaskSchedulerContext>(options => options.UseSqlServer(connectionString));
        }
        public static void AddSerilogLogger(this IServiceCollection services, IConfiguration configuration)
        {
            var loggerDangerMessageProvider = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration, sectionName: "SerilogDangerMessages")
                .Enrich.FromLogContext()
                .CreateLogger();

            //var loggerAllMessageProvider = new LoggerConfiguration()
            //    .ReadFrom.Configuration(configuration, sectionName: "SerilogAllMessages")
            //    .Enrich.FromLogContext()
            //    .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.AddSerilog(loggerDangerMessageProvider);
                //builder.AddSerilog(loggerAllMessageProvider);
            });
        }
        public static void AddCookie(this IServiceCollection services)
        {
            services.AddAuthentication(WebCommon.CookieName).AddCookie(WebCommon.CookieName, options =>
            {
                options.Cookie.Name = WebCommon.CookieName;
            });
        }
    }
}