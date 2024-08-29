
using AutoMapper;
using AbyKhedma.Dtos;
using AbyKhedma.Entities;
using AbyKhedma.Core.Models;
using Core.Dtos;
using Core.Models;


namespace AbyKhedma.Mappers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //CreateMap<User, UserForDetailedDto>()
            //    .ForMember(
            //    dest=> dest.PhotoUrl,
            //    opt=> opt.MapFrom(
            //        src=>src.Photos.FirstOrDefault(p=>p.IsMain).Url 
            //        ))
            //    .ForMember(
            //    dest => dest.Age ,
            //    opt => opt.MapFrom(
            //        src => src.DateOfBirth.CalculateAge()
            //        ));
            CreateMap<RequesterUserForRegisterDto, User>();
            CreateMap<EmployeeUserForRegisterDto, User>();
            CreateMap<User, UserModel>();
            CreateMap<UserForEditDto, User>()
                .ForMember(dest => dest.UpdatedBy, act => act.Ignore())
                .ForMember(dest => dest.CreatedBy, act => act.Ignore())
                .ForMember(dest => dest.CreatedDate, act => act.Ignore())
                .ForMember(dest => dest.UpdatedDate, act => act.Ignore());


            CreateMap<CategoryToCreateDto, Category>().ReverseMap();
            CreateMap<SubCategoryToCreateDto, Category>().ReverseMap();

            //.ForMember(
            //dest => dest.ParentCategoryId,
            //opt => opt.Ignore())
            //.ForMember(
            //dest => dest.ClosingStatementId,
            //opt => opt.Ignore())
            //.ForMember(
            //dest => dest.OpeningStatementId,
            //opt => opt.Ignore())
            //.ForMember(
            //dest => dest.HasChilds,
            //opt => opt.Ignore())
            //.ForMember(
            //dest => dest.Url,
            //opt => opt.Ignore())
            //.ForMember(
            //dest => dest.UrlPublicId,
            //opt => opt.Ignore());

            CreateMap<Category, CategoryModel>()
                .ForMember(
                dest => dest.SubCategories,
                opt => opt.Ignore()).ReverseMap();
            CreateMap<Service, ServiceModel>().ReverseMap();
            CreateMap<Service, ServiceToCreateDto>().ReverseMap();
            CreateMap<Requirement, RequirementModel>().ReverseMap();
            CreateMap<Requirement, RequirementToCreateDto>().ReverseMap();
            CreateMap<RequirementToEditDto, Requirement>()
                .ForMember(dest => dest.UpdatedBy, act => act.Ignore())
                .ForMember(dest => dest.CreatedBy, act => act.Ignore())
                .ForMember(dest => dest.CreatedDate, act => act.Ignore())
                .ForMember(dest => dest.UpdatedDate, act => act.Ignore());

            CreateMap<Answer, AnswerDto>().ReverseMap();
            CreateMap<Statement, StatementModel>().ReverseMap();
            CreateMap<Statement, StatementToCreateDto>().ReverseMap();
            CreateMap<Reel, ReelToCreateDto>().ReverseMap();
            CreateMap<Reel, ReelModel>().ReverseMap();
            CreateMap<RequestForEditDto, Request>()
                .ForMember(dest => dest.UpdatedBy, act => act.Ignore())
                .ForMember(dest => dest.CreatedBy, act => act.Ignore())
                .ForMember(dest => dest.CreatedDate, act => act.Ignore())
                .ForMember(dest => dest.UpdatedDate, act => act.Ignore());

            CreateMap<Request, RequestModel>().ReverseMap();
            CreateMap<Request, RequestHistoryDto>().ReverseMap();

            CreateMap<RequestFlow, RequestFlowModel>().ReverseMap();
            CreateMap<AuditLogModel, AuditLog>().ReverseMap();



            CreateMap<RequestFlow, RequestFlowToCreateDto>().ReverseMap();
            CreateMap<ChatMessage, ChatMessageToCreateDto>().ReverseMap();
            CreateMap<ChatMessage, ChatMessageModel>().ReverseMap();
            CreateMap<Message, MessageModel>().ReverseMap();

            
            CreateMap<Attachment, AttachmentDto>().ReverseMap();
            CreateMap<UserDto, User>().ReverseMap();


            CreateMap<SubAnswerRequirement, SubAnswerRequirementDto>().ReverseMap();
            CreateMap<SubAnswerRequirementToCreateDto, SubAnswerRequirement>().ReverseMap();


        }
    }
}
