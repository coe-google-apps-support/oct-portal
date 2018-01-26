using AutoMapper;
using CoE.Ideas.Core.ProjectManagement;
using CoE.Ideas.ProjectManagement.Core;
using CoE.Ideas.ProjectManagement.Core.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.ProjectManagement.Tests
{
    public class TestConfiguration
    {
        public TestConfiguration(IConfigurationRoot configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException("configuration");
            _services = new ServiceCollection();
        }

        public TestConfiguration(IConfigurationRoot configuration,
            IServiceCollection services)
        {
            _configuration = configuration ?? throw new ArgumentNullException("configuration");
            _services = services ?? throw new ArgumentNullException("services");
        }

        private readonly IConfigurationRoot _configuration;
        private readonly IServiceCollection _services;

        public ServiceProvider ServiceProvider { get; private set; }

        public ServiceProvider BuildServiceProvider()
        {
            ServiceProvider = _services.BuildServiceProvider();
            return ServiceProvider;
        }

        public TestConfiguration ConfigureBasicServices()
        {
            // basic stuff - there's probably a better way to register these
            _services.AddSingleton(
                typeof(Microsoft.Extensions.Options.IOptions<>),
                typeof(Microsoft.Extensions.Options.OptionsManager<>));
            _services.AddSingleton(
                typeof(Microsoft.Extensions.Options.IOptionsFactory<>),
                typeof(Microsoft.Extensions.Options.OptionsFactory<>));

            return this;
        }

        public TestConfiguration ConfigureProjectManagementServices()
        {
            _services.AddProjectManagementConfiguration(
                _configuration.GetConnectionString("IdeaProjectManagementDatabase"));

            _services.AddAutoMapper();
            return this;
        }

        internal TestConfiguration ConfigureProjectManagementServicesInMemory()
        {
            _services.AddDbContext<ProjectManagementContext>(x => x.UseInMemoryDatabase("pm"));
            _services.AddDbContext<ExtendedProjectManagementContext>(x => x.UseInMemoryDatabase("pm"));
            _services.AddScoped<IProjectManagementRepository, ProjectManagementRepositoryInternal>();
            _services.AddScoped<IExtendedProjectManagementRepository, ExtendedProjectManagementRepositoryInternal>();


            _services.AddAutoMapper();
            return this;
        }
    }
}
