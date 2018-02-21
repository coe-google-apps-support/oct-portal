using AutoMapper.Configuration;
using CoE.Ideas.Core.Internal;
using CoE.Ideas.Core.Internal.Initiatives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoE.Ideas.Core
{
    internal class MappingProfile : MapperConfigurationExpression
    {
        public MappingProfile() : base()
        {
            CreateMap<Idea, IdeaInternal>();
            CreateMap<IdeaInternal, Idea>()
                .ForMember(i => i.Owner, x => x.MapFrom(y => y.Stakeholders.FirstOrDefault(z => z.Type == StakeholderTypeInternal.Owner).Person));
            CreateMap<Person, PersonInternal>();
            CreateMap<PersonInternal, Person>();
            CreateMap<Stakeholder, StakeholderInternal>();
            CreateMap<StakeholderInternal, Stakeholder>()
                .ForMember(s => s.Email, x => x.MapFrom(si => si.Person.Email))
                .ForMember(s => s.UserName, x => x.MapFrom(si => si.Person.UserName));
            CreateMap<StakeholderType, StakeholderTypeInternal>();
            CreateMap<StakeholderTypeInternal, StakeholderType>();
            CreateMap<InitiativeStatus, InitiativeStatusInternal>();
            CreateMap<InitiativeStatusInternal, InitiativeStatus>();
            CreateMap<Tag, TagInternal>();
            CreateMap<TagInternal, Tag>();
            CreateMap<Branch, BranchInternal>();
            CreateMap<BranchInternal, Branch>();
            CreateMap<Department, DepartmentInternal>();
            CreateMap<DepartmentInternal, Department>();
        }
    }
}
