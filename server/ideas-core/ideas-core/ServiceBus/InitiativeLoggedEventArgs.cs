namespace CoE.Ideas.Core.ServiceBus
{
    public class InitiativeLoggedEventArgs : InitiativeCreatedEventArgs
    {
        public string RangeUpdated { get; set; }
    }
}