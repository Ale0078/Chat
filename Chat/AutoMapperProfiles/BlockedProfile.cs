using AutoMapper;

using Chat.Entities;
using Chat.Models;

namespace Chat.Server.AutoMapperProfiles
{
    public class BlockedProfile : Profile
    {
        public BlockedProfile()
        {
            CreateMap<BlockedUser, BlockModel>()
                .ForMember(
                    destinationMember: block => block.UserId,
                    memberOptions: options => options.MapFrom(
                        mapExpression: blockedUser => blockedUser.Blocker.UserId))
                .ForMember(
                    destinationMember: block => block.DoesBlocked,
                    memberOptions: options => options.MapFrom(
                        mapExpression: blockedUser => blockedUser.Blocker.DoesBlock));
        }
    }
}
