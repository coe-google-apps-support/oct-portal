using CoE.Ideas.Core;
using CoE.Ideas.Core.WordPress;
using RemedyService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Remedy
{
    internal class RemedyServiceImpl : IRemedyService
    {
        public RemedyServiceImpl(New_Port_0PortType remedyClient)
        {
            _remedyClient = remedyClient;
        }

        private readonly New_Port_0PortType _remedyClient;



        public async Task PostNewIdeaAsync(Idea idea, string user3and3)
        {
            var request = new New_Create_Operation_0Request();

            // TODO: fill request parameters
            // ...


            await _remedyClient.New_Create_Operation_0Async(request);
        }
    }
}
