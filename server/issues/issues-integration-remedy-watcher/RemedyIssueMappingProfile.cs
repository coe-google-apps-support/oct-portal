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
            map.ForMember(issue => issue.Title, opt => opt.MapFrom(r => r.Description));
            map.ForMember(issue => issue.Description, opt => opt.MapFrom(r => r.Detailed_Decription));
            map.ForMember(issue => issue.CreatedDate, opt => opt.MapFrom(r => r.Submit_Date));
            map.ForMember(issue => issue.RemedyStatus, opt => opt.MapFrom(r => r.Status));
            map.ForMember(issue => issue.ReferenceId, opt => opt.MapFrom(r => r.Incident_Number));
            map.ForMember(issue => issue.Urgency, opt => opt.MapFrom(r => r.Urgency));
        }
    }
}
