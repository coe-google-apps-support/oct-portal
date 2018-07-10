using AutoMapper;
using CoE.Issues.Core.ServiceBus;
using CoE.Issues.Remedy.Watcher.RemedyServiceReference;

namespace CoE.Issues.Remedy.Watcher
{
    public class RemedyIssueMappingProfile : Profile
    {
        public RemedyIssueMappingProfile()
        {

            var map = CreateMap<OutputMapping1GetListValues, IssueCreatedEventArgs>();
            map.ForMember(issue => issue.Title, opt => opt.MapFrom(r => r.Short_Description));
            map.ForMember(issue => issue.Description, opt => opt.MapFrom(r => r.Description));

            //TODO: map the rest of the properties and add more properties to issue
        }
    }
}
