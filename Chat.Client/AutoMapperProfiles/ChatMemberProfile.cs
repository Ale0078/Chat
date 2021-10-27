using System.Linq;
using System.Collections.ObjectModel;
using AutoMapper;

using Chat.Client.ViewModel;
using Chat.Models;

namespace Chat.Client.AutoMapperProfiles
{
    public class ChatMemberProfile : Profile
    {
        public ChatMemberProfile()
        {
            CreateMap<FullUserModel, ChatMemberViewModel>();
            CreateMap<ChatMemberViewModel, FullUserModel>();

            IMapper mapper = GetMessageMapper();

            CreateMap<UserModel, ChatMemberViewModel>()
                .ForMember(
                    destinationMember: chatMember => chatMember.Messages,
                    memberOptions: options => options.MapFrom(
                        mapExpression: user => mapper.Map<ObservableCollection<ChatMessageViewModel>>(user.Messages)))
                .ForMember(
                    destinationMember: chatMember => chatMember.LastMessage,
                    memberOptions: options => options.MapFrom(
                        mapExpression: user => user.Messages.Count == 0
                            ? new ChatMessageViewModel()
                            : mapper.Map<ChatMessageViewModel>(user.Messages.Last())));

            CreateMap<ChatMemberViewModel, UserModel>()
                .ForMember(
                    destinationMember: user => user.Messages,
                    memberOptions: options => options.MapFrom(
                        mapExpression: chatMember => mapper.Map<ObservableCollection<ChatViewModel>>(chatMember.Messages)));

            CreateMap<GroupUser, GroupUserViewModel>();
            CreateMap<GroupUserViewModel, GroupUser>();

            CreateMap<GroupUserViewModel, ChatMemberViewModel>();
            CreateMap<ChatMemberViewModel, GroupUserViewModel>();
        }

        private IMapper GetMessageMapper() =>
            new Mapper(new MapperConfiguration(options => 
            {
                options.CreateMap<ChatMessageModel, ChatMessageViewModel>();
                options.CreateMap<ChatMessageViewModel, ChatMessageModel>();
            }));
    }
}
