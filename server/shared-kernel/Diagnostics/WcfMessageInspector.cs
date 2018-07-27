using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;

namespace CoE.Shared.Diagnostics
{
    internal class WcfMessageInspector : IClientMessageInspector
    {
        public WcfMessageInspector(Serilog.ILogger logger)
        {
            _logger = logger;
        }
        private readonly Serilog.ILogger _logger;

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
           // _logger.Information("About to send to {Url}: {Body}", channel.RemoteAddress.Uri, request.ToString());
            return null;
        }
    }

    internal class WcfMessageInspectorEndpointBehavior : IEndpointBehavior
    {
        public WcfMessageInspectorEndpointBehavior(Serilog.ILogger logger)
        {
            _logger = logger;
        }
        private readonly Serilog.ILogger _logger;


        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new WcfMessageInspector(_logger));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}
