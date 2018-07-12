namespace CoE.Issues.Remedy.Watcher
{
    public class RemedyReaderOptions
    {
        /// <summary>
        /// Gets or sets the maximum number of concurrent calls to the callback the message
        /// pump should initiate.
        /// </summary>
        public int MaxConcurrentCalls { get; set; }
        public string ConnectionString { get; set; }
    }
}