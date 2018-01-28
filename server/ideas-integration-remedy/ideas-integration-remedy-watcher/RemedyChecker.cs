using Microsoft.Azure.ServiceBus;
using RemedyServiceReference;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Remedy.Watcher
{
    public class RemedyChecker
    {
        public RemedyChecker(New_Port_0PortTypeClient remedyClient,
            AuthenticationInfo remedyAuthenticationInfo,
            ITopicClient topiClient)
        {
            _remedyClient = remedyClient ?? throw new ArgumentNullException("remedyClient");
            _remedyAuthenticationInfo = remedyAuthenticationInfo ?? throw new ArgumentNullException("remedyAuthenticationInfo");
            _topicClient = topiClient ?? throw new ArgumentNullException("topiClient");
        }

        private readonly New_Port_0PortTypeClient _remedyClient;
        private readonly AuthenticationInfo _remedyAuthenticationInfo;
        private readonly ITopicClient _topicClient;
        private readonly string _templateName = "Work Order - Generic - Create"; // TODO; get from configuration

        private DateTime lastPollTimeUtc;

        public void Poll()
        {
            New_Get_Operation_0Response remedyResponse;
            try
            {
                remedyResponse = await _remedyClient.New_Get_Operation_0Async(_remedyAuthenticationInfo, _templateName, lastPollTimeUtc.ToLongTimeString());
                lastPollTimeUtc = DateTime.Now.ToUniversalTime();
            }
            catch (Exception err)
            {

            }
            finally
            {

            }


        }
    }
}
