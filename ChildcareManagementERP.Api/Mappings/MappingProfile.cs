using AutoMapper;
using ChildcareManagementERP.Api.Dtos.V1;
using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

namespace ChildcareManagementERP.Api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Guardian, GuardianDto>();
            CreateMap<Guardian, GuardianWithFinancialInformationDto>();
            CreateMap<Guardian, GuardianWithPersonalSituationDto>();
            CreateMap<Guardian, GuardianWithChildrenDto>()
                .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.GuardianChildren.Select(gc => gc.Child)));
            CreateMap<CreateGuardianDto, Guardian>();
            CreateMap<UpdateGuardianDto, Guardian>();

            CreateMap<Child, ChildDto>();
            CreateMap<Child, ChildWithGuardiansDto>()
                .ForMember(dest => dest.Guardians, opt => opt.MapFrom(src => src.GuardianChildren.Select(gc => gc.Guardian)));
            CreateMap<Child, ChildWithAuthorizedPeopleDto>()
                .ForMember(dest => dest.AuthorizedPeople, opt => opt.MapFrom(src => src.AuthorizedPersonChildren.Select(apc => apc.AuthorizedPerson)));
            CreateMap<Child, ChildWithAdditionalDatasDto>();
            CreateMap<CreateChildDto, Child>();
            CreateMap<UpdateChildDto, Child>();

            CreateMap<FinancialInformation, FinancialInformationDto>();
            CreateMap<CreateFinancialInformationDto, FinancialInformation>();
            CreateMap<UpdateFinancialInformationDto, FinancialInformation>();

            CreateMap<PersonalSituation, PersonalSituationDto>();
            CreateMap<CreatePersonalSituationDto, PersonalSituation>();
            CreateMap<UpdatePersonalSituationDto, PersonalSituation>();

            CreateMap<AdditionalData, AdditionalDataDto>();
            CreateMap<CreateAdditionalDataDto, AdditionalData>();
            CreateMap<UpdateAdditionalDataDto, AdditionalData>();

            CreateMap<AuthorizedPerson, AuthorizedPersonDto>();
            CreateMap<AuthorizedPerson, AuthorizedPersonWithChildrenDto>()
                .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.AuthorizedPersonChildren.Select(apc => apc.Child)));
            CreateMap<CreateAuthorizedPersonDto, AuthorizedPerson>();
            CreateMap<UpdateAuthorizedPersonDto, AuthorizedPerson>();

            CreateMap<GuardianChild, LinkGuardianChildDto>();
            CreateMap<CreateLinkGuardianChildDto, GuardianChild>();
            CreateMap<UpdateLinkGuardianChildDto, GuardianChild>();

            CreateMap<AuthorizedPersonChild, LinkAuthorizedPersonChildDto>();
            CreateMap<CreateLinkAuthorizedPersonChildDto, AuthorizedPersonChild>();
            CreateMap<UpdateLinkAuthorizedPersonChildDto, AuthorizedPersonChild>();
        }
    }
}