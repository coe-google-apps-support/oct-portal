using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Remedy.Listener.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;

namespace CoE.Ideas.Remedy.Listener.Web.Controllers
{
    [Route("Remedy/[controller]")]
    public class StatusController : Controller
    {
        public StatusController(ITopicClient serviceBusTopicClient)
        {
            _serviceBusTopicClient = serviceBusTopicClient ?? throw new ArgumentNullException("serviceBusTopicClient");
        }

        private readonly ITopicClient _serviceBusTopicClient;

        // POST remedy/status
        [HttpPost]
        public async Task Post([FromBody]StatusUpdate update)
        {
            var message = new Message
            {
                Label = "Remedy"
            };
            message.UserProperties.Add("Event", "StatusUpdated");
            message.UserProperties.Add("WorkItemId", update.WorkOrderId);
            message.UserProperties.Add("WorkItemStatus", update.Status);

            await _serviceBusTopicClient.SendAsync(message);
        }
    }
}
