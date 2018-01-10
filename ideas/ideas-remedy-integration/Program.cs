using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using System.Threading;

namespace CoE.Ideas.Remedy
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: get settings here from configuration
            var ideaQueueSettings = IdeaFactory.GetIdeaQueue();
            var wordPressClient = IdeaFactory.GetWordPressClient();
            var ideaRepository = IdeaFactory.GetIdeaRepository();

            // Register listener
            new IdeaQueue(ideaQueueSettings)
                .RegisterIdeaListener(new NewIdeaListener(ideaRepository, wordPressClient));

            // now block forever
            // but I don't think the code will ever get here anyway...
            new ManualResetEvent(false).WaitOne();
        }
    }
}
