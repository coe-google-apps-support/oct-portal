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


        //// GET api/values
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST remedy/status
        [HttpPost]
        public async Task Post([FromBody]StatusUpdate update)
        {
            var message = new Message
            {
                Label = "Remedy"
            };
            message.UserProperties.Add("Event", "StatusUpdated");
            message.UserProperties.Add("WorkOrderId", update.WorkOrderId);
            message.UserProperties.Add("WorkOrderStatus", update.Status);

            await _serviceBusTopicClient.SendAsync(message);
        }

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
