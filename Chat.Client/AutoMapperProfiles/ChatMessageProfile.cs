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
        }
    }
}
