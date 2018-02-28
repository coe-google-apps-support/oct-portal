using AutoMapper;
using CoE.Ideas.Core.Internal.WordPress;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CoE.Ideas.Core.Internal.Initiatives
{
    internal class IdeaRepositoryFactory : IIdeaRepositoryFactory
    {
        public IdeaRepositoryFactory(
            IServiceProvider serviceProvider,
            //IWordPressUserSecurity wordPressUserSecurity,
            IOptions<IdeaRepositoryFactoryOptions> options
            //Serilog.ILogger logger)
            )
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException("serviceProvider");
            //_wordPressUserSecurity = wordPressUserSecurity ?? throw new ArgumentNullException("wordPressUserSecurity");
            //_logger = logger ?? throw new ArgumentNullException("logger");
            _options = options ?? throw new ArgumentNullException("options");
        }

        private readonly IOptions<IdeaRepositoryFactoryOptions> _options;

        private readonly IServiceProvider _serviceProvider;

        public IIdeaRepository Create(ClaimsPrincipal user)
        {
            if (_options.Value.IsRemote)
                return new RemoteIdeaRepository(user,
                    _serviceProvider.GetService(typeof(IWordPressUserSecurity)) as IWordPressUserSecurity,
                    _serviceProvider.GetService(typeof(IOptions<RemoteIdeaRepositoryOptions>)) as IOptions<RemoteIdeaRepositoryOptions>,
                    _serviceProvider.GetService(typeof(Serilog.ILogger)) as Serilog.ILogger);
            else
                return new IdeaRepositoryInternal(_serviceProvider.GetService(typeof(IdeaContext)) as IdeaContext,
                    _serviceProvider.GetService(typeof(IMapper)) as IMapper,
                    _serviceProvider.GetService(typeof(ILogger<IdeaRepositoryInternal>)) as ILogger<IdeaRepositoryInternal>);
        }
    }
}
