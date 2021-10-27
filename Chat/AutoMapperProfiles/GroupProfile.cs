using AutoMapper;

using Chat.Models;
using Chat.Entities;

namespace Chat.Server.AutoMapperProfiles
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<Group, GroupModel>();
            CreateMap<GroupModel, Group>();
        }
    }
}
