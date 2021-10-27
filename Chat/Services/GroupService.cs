using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using AutoMapper;

using Chat.Server.Services.Interfaces;
using Chat.Entities.Contexts;
using Chat.Entities;
using Chat.Models;

namespace Chat.Server.Services
{
    public class GroupService : IGroupService
    {
        private readonly ApplicationContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public GroupService(ApplicationContext dbContext, UserManager<User> userManager, IMapper mapper)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _mapper = mapper;
        }

        public Task<List<string>> GetListOfGroupNamesByUserNameAsync(string userName)
        {
            List<Group> groups = _dbContext.Groups.ToList();

            List<string> groupNames = new();

            foreach (Group group in groups)
            {
                if (group.Users.Exists(user => user.UserName == userName))
                {
                    groupNames.Add(group.Name);
                }
            }

            return Task.FromResult(groupNames);
        }

        public async Task<GroupModel> CreateGroupAsync(string groupName, byte[] groupPhoto, List<GroupUser> users) 
        {
            if (_dbContext.Groups.Where(group => group.Name == groupName).Count() != 0)
            {
                throw new InvalidOperationException("This chat has already exist");
            }

            List<User> groupUsers = new(users.Count);

            foreach (User user in _dbContext.Users)
            {
                if (users.Exists(groupUser => groupUser.Id == user.Id))
                {
                    groupUsers.Add(user);
                }
            }

            EntityEntry<Group> group = await _dbContext.Groups.AddAsync(new Group
            {
                Name = groupName,
                Photo = groupPhoto,
                Users = groupUsers
            });

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<GroupModel>(group.Entity);
        }

        public async Task<GroupMessageModel> CreateGroupMessageAsync(GroupMessageModel message) 
        {
            EntityEntry<GroupMessage> groupMessage = await _dbContext.GroupMessages.AddAsync(_mapper.Map<GroupMessage>(message));

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<GroupMessageModel>(groupMessage.Entity);
        }

        public async Task<bool> AddGroupUserToGroup(GroupUser user, string groupName)
        {
            Group group = _dbContext.Groups.FirstOrDefault(group => group.Name == groupName);

            if (group is null)
            {
                return false;
            }

            group.Users.Add(_mapper.Map<User>(user));

            _dbContext.Update(group);

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveGroupUserFromGroup(GroupUser user, string groupName)
        {
            Group group = _dbContext.Groups.FirstOrDefault(group => group.Name == groupName);

            if (group is null)
            {
                return false;
            }

            group.Users.Remove(
                item: group.Users.Find(entityUser => entityUser.UserName == user.Name));

            _dbContext.Update(group);

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
