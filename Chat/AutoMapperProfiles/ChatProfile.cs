using AutoMapper;

using Chat.Entities;
using Chat.Models;

namespace Chat.Server.AutoMapperProfiles
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<Chatter, ChatModel>()
                .ForMember(
                    destinationMember: chatModel => chatModel.Id,
                    memberOptions: options => options.MapFrom(
                        mapExpression: chatter => chatter.ChatId))
                .ForMember(
                    destinationMember: chatModel => chatModel.FirstUserId,
                    memberOptions: options => options.MapFrom(
                        mapExpression: chatter => chatter.Chat.FirstUserId))
                .ForMember(
                    destinationMember: chatModel => chatModel.SecondUserId,
                    memberOptions: options => options.MapFrom(
                        mapExpression: chatter => chatter.Chat.SecondUserId));
        }
    }
}
