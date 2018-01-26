using AutoMapper;
using CoE.Ideas.Core.ProjectManagement;
using CoE.Ideas.ProjectManagement.Core.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.ProjectManagement.Core
{
    /// <summary>
    /// Extension methods for setting up MVC services in an Microsoft.Extensions.DependencyInjection.IServiceCollection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectManagementConfiguration(
            this IServiceCollection services,
            string dbConnectionString)
        {
            if (string.IsNullOrWhiteSpace(dbConnectionString))
                throw new ArgumentNullException("dbConnectionString");

            services.AddDbContext<ExtendedProjectManagementContext>(options =>
                options.UseMySql(dbConnectionString));

            services.AddDbContext<ProjectManagementContext>(options =>
                options.UseMySql(dbConnectionString));



            services.AddScoped<IExtendedProjectManagementRepository, ExtendedProjectManagementRepositoryInternal>();
            services.AddScoped<IProjectManagementRepository, ProjectManagementRepositoryInternal>();

            services.AddAutoMapper();

            return services;
        }
    }
}
