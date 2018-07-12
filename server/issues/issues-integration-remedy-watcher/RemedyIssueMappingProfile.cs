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
            map.ForMember(issue => issue.Title, opt => opt.MapFrom(r => r.First_Name));
            map.ForMember(issue => issue.Description, opt => opt.MapFrom(r => r.Detailed_Decription));
            map.ForMember(issue => issue.CreatedDate, opt => opt.MapFrom(r => r.Submit_Date));
            map.ForMember(issue => issue.AssigneeEmail, opt => opt.MapFrom(r => r.z1D_Assignee_Email));
            map.ForMember(issue => issue.RequestorName, opt => opt.MapFrom(r => r.Full_Name));
            map.ForMember(issue => issue.RemedyStatus, opt => opt.MapFrom(r => r.z1D_DisplayStatusReason));
            map.ForMember(issue => issue.ReferenceId, opt => opt.MapFrom(r => r.Incident_Number));

            //TODO: map the rest of the properties and add more properties to issue
        }
    }
}
