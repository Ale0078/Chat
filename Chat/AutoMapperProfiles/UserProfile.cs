using System;
using AutoMapper;

using Chat.Entities;
using Chat.Models;

namespace Chat.Server.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, FullUserModel>()
                .ForMember(
                    destinationMember: fullUser => fullUser.Name,
                    memberOptions: options => options.MapFrom(
                        mapExpression: user => user.UserName))
                .ForMember(
                    destinationMember: fullUser => fullUser.IsAdmin,
                    memberOptions: options => options.MapFrom(
                        mapExpression: user => user.UserName == "Admin"))
                .ForMember(
                    destinationMember: fullUser => fullUser.BlockModels,
                    memberOptions: options => options.MapFrom(
                        mapExpression: user => user.BlockedUsers));

            CreateMap<RegisterUserModel, User>()
                .ForMember(
                    destinationMember: user => user.DisconnectTime,
                    memberOptions: options => options.MapFrom(
                        mapExpression: registerModel => DateTime.Now));

            CreateMap<User, UserModel>()
                .ForMember(
                    destinationMember: userModel => userModel.Name,
                    memberOptions: options => options.MapFrom(
                        mapExpression: user => user.UserName));

            CreateMap<User, GroupUser>()
                .ForMember(
                    destinationMember: groupUser => groupUser.Name,
                    memberOptions: options => options.MapFrom(
                        mapExpression: user => user.UserName));

            CreateMap<GroupUser, User>()
                .ForMember(
                    destinationMember: user => user.UserName,
                    memberOptions: options => options.MapFrom(
                        mapExpression: groupUser => groupUser.Name));
        }
    }
}
