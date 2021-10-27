using AutoMapper;

using Chat.Models;
using Chat.Client.ViewModel;

namespace Chat.Client.AutoMapperProfiles
{
    public class ChatGroupProfile : Profile
    {
        public ChatGroupProfile()
        {
            CreateMap<GroupModel, GroupViewModel>()
                .ForMember(
                    destinationMember: groupViewModel => groupViewModel.Messages,
                    memberOptions: options => options.MapFrom(
                        mapExpression: groupModel => groupModel.GroupMessages));

            CreateMap<GroupViewModel, GroupModel>()
                .ForMember(
                    destinationMember: groupModel => groupModel.GroupMessages,
                    memberOptions: options => options.MapFrom(
                        mapExpression: groupViewModel => groupViewModel.Messages));
        }
    }
}
