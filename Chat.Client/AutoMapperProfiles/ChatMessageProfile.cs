using AutoMapper;

using Chat.Client.ViewModel;
using Chat.Models;

namespace Chat.Client.AutoMapperProfiles
{
    public class ChatMessageProfile : Profile
    {
        public ChatMessageProfile()
        {
            CreateMap<ChatMessageModel, ChatMessageViewModel>();
            CreateMap<ChatMessageViewModel, ChatMessageModel>();

            CreateMap<GroupMessageModel, GroupMessageViewModel>()
                .ForMember(
                    destinationMember: messageViewModel => messageViewModel.Message,
                    memberOptions: options => options.MapFrom(
                        mapExpression: messageModel => messageModel.TextMessage))
                .ForMember(
                    destinationMember: messageViewModel => messageViewModel.ByteFile,
                    memberOptions: options => options.MapFrom(
                        mapExpression: messageModel => messageModel.FileMessage))
                .ForMember(
                    destinationMember: messageViewModel => messageViewModel.FromUserId,
                    memberOptions: options => options.MapFrom(
                        mapExpression: messageModel => messageModel.SenderId));

            CreateMap<GroupMessageViewModel, GroupMessageModel>()
                .ForMember(
                    destinationMember: messageModel => messageModel.TextMessage,
                    memberOptions: options => options.MapFrom(
                        mapExpression: messageViewModel => messageViewModel.Message))
                .ForMember(
                    destinationMember: messageModel => messageModel.FileMessage,
                    memberOptions: options => options.MapFrom(
                        mapExpression: messageViewModel => messageViewModel.ByteFile))
                .ForMember(
                    destinationMember: messageModel => messageModel.SenderId,
                    memberOptions: options => options.MapFrom(
                        mapExpression: messageViewModel => messageViewModel.FromUserId));
        }
    }
}
