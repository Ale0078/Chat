using AutoMapper;

using Chat.Entities;
using Chat.Models;

namespace Chat.Server.AutoMapperProfiles
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<ChatMessageModel, ChatMessage>()
                .ForMember(
                    destinationMember: message => message.ReceiverId,
                    memberOptions: options => options.MapFrom(
                        mapExpression: messageModel => messageModel.ToUserId))
                .ForMember(
                    destinationMember: message => message.SenderId,
                    memberOptions: options => options.MapFrom(
                        mapExpression: messageModel => messageModel.FromUserId))
                .ForMember(
                    destinationMember: message => message.Time,
                    memberOptions: options => options.MapFrom(
                        mapExpression: messageModel => messageModel.SendingTime));

            CreateMap<ChatMessage, ChatMessageModel>()
                .ForMember(
                    destinationMember: messageModel => messageModel.ToUserId,
                    memberOptions: options => options.MapFrom(
                        mapExpression: message => message.ReceiverId))
                .ForMember(
                    destinationMember: messageModel => messageModel.FromUserId,
                    memberOptions: options => options.MapFrom(
                        mapExpression: message => message.SenderId))
                .ForMember(
                    destinationMember: messageModel => messageModel.SendingTime,
                    memberOptions: options => options.MapFrom(
                        mapExpression: message => message.Time));

            CreateMap<GroupMessageModel, GroupMessage>();
            CreateMap<GroupMessage, GroupMessageModel>();
        }
    }
}
