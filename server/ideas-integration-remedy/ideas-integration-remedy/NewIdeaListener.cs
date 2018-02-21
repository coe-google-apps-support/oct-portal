using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace CoE.Ideas.Remedy
{
    public class NewIdeaListener
    {
        public NewIdeaListener(IInitiativeMessageReceiver initiativeMessageReceiver,
            IInitiativeMessageSender initiativeMessageSender,
            IRemedyService remedyService,
            //IActiveDirectoryUserService activeDirectoryUserService,
            Serilog.ILogger logger) 
        {
            _initiativeMessageReceiver = initiativeMessageReceiver ?? throw new ArgumentNullException("initiativeMessageReceiver");
            _initiativeMessageSender = initiativeMessageSender ?? throw new ArgumentNullException("initiativeMessageSender");
            _remedyService = remedyService ?? throw new ArgumentNullException("remedyService");
            //_activeDirectoryUserService = activeDirectoryUserService ?? throw new ArgumentNullException("activeDirectoryUserService");
            _logger = logger ?? throw new ArgumentNullException("logger");

            _logger.Information("Starting messsage pump for New Initiatives");
            initiativeMessageReceiver.ReceiveMessages(initiativeCreatedHandler: OnNewInitiative);

        }

        private readonly IInitiativeMessageReceiver _initiativeMessageReceiver;
        private readonly IInitiativeMessageSender _initiativeMessageSender;
        private readonly IRemedyService _remedyService;
        //private readonly IActiveDirectoryUserService _activeDirectoryUserService;
        private readonly Serilog.ILogger _logger;

        protected virtual Task OnError(ExceptionReceivedEventArgs err)
        {
            _logger.Error(err.Exception, "Error receiving message: {ErrorMessage}", err.Exception.Message);
            return Task.CompletedTask;
        }

        protected virtual async Task OnNewInitiative(InitiativeCreatedEventArgs e, CancellationToken token)
        {
            var initiative = e.Initiative;
            var owner = e.Owner;
            using (LogContext.PushProperty("InitiativeId", initiative.Id))
            {
                _logger.Information("Begin OnNewInitiative");
                Stopwatch watch = new Stopwatch();
                watch.Start();

                string user3and3 = await GetUser3and3(owner.GetEmail());
                string workOrderId = await CreateWorkOrder(initiative, user3and3);
                await SendWorkOrderCreatedMessage(initiative, owner, workOrderId);

                _logger.Information("Processed OnNewInitiative in {ElapsedMilliseconds}ms", watch.ElapsedMilliseconds);
            }
        }

        protected virtual async Task<string> GetUser3and3(string email)
        {
            //Stopwatch watch = new Stopwatch();
            //UserPrincipal adUser = null;
            //try
            //{
            //    //    adUser = _activeDirectoryUserService.GetADUser(wordPressUser.Email);
            //}
            ////catch (Exception err)
            ////{
            ////    throw new InvalidOperationException($"Unable to find an Active Directory user with email { wordPressUser.Email }: { err.Message }");
            ////}

            ////if (adUser == null)
            ////    throw new InvalidOperationException($"Unable to find an Active Directory user with email { wordPressUser.Email }");
            //finally
            //{
            //    watch.Restart();
            //}
            // return adUser?.SamAccountName;

            return await Task.FromResult<string>(null);
        }

        protected virtual async Task<string> CreateWorkOrder(Idea initiative, string user3And3)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            string remedyTicketId = null;
            remedyTicketId = await _remedyService.PostNewIdeaAsync(initiative, user3And3);
            _logger.Information("Created Remedy Work Order in {ElapsedMilliseconds}ms. Initiative Id { InitiativeId }, WorkOrderId { WorkOrderId }", watch.ElapsedMilliseconds, initiative.Id, remedyTicketId);
            return remedyTicketId;
        }

        protected virtual async Task SendWorkOrderCreatedMessage(Idea initiative, ClaimsPrincipal owner, string workOrderId)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            await _initiativeMessageSender.SendInitiativeWorkOrderCreatedAsync(
                new WorkOrderCreatedEventArgs()
                {
                    Initiative = initiative,
                    Owner = owner,
                    WorkOrderId = workOrderId
                });

            _logger.Information("Send remedy work order created message to service bus in {ElapsedMilliseconds}ms", watch.ElapsedMilliseconds);
        }
    }
}
