using AutoMapper;
using CoE.Issues.Core.ServiceBus;
using CoE.Issues.Remedy.WorkOrder.Watcher.RemedyServiceReference;

namespace CoE.Issues.Remedy.WorkOrder.Watcher
{
    public class RemedyIssueMappingProfile : Profile
    {
        public RemedyIssueMappingProfile()
        {

            var map = CreateMap<OutputMapping1GetListValues, IssueCreatedEventArgs>();
            map.ForMember(issue => issue.Title, opt => opt.MapFrom(r => r.Description));
            map.ForMember(issue => issue.Description, opt => opt.MapFrom(r => r.Detailed_Description));
            map.ForMember(issue => issue.CreatedDate, opt => opt.MapFrom(r => r.Submit_Date));
            map.ForMember(issue => issue.RemedyStatus, opt => opt.MapFrom(r => r.Status));
            map.ForMember(issue => issue.ReferenceId, opt => opt.MapFrom(r => r.WorkOrderID));
            map.ForMember(issue => issue.Urgency, opt => opt.MapFrom(r => r.Priority));

            //TODO: map the rest of the properties and add more properties to issue
        }
    }
}
