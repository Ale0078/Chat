using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Threading;
using AutoMapper;

using Chat.Client.Services;
using Chat.Client.Commands;
using Chat.Models;
using Chat.Client.Extensions;
using Chat.Client.ViewModel.Base;

namespace Chat.Client.ViewModel
{
    public class ChatViewModel : ViewModelBase
    {
        private readonly IMapper _mapper;
        private readonly ChatService _chatService;
        private readonly ChatGroupService _chatGroupService;
        private readonly RegistrationChatService _registrationChatService;
        private readonly List<ChatMemberViewModel> _blockedUsers;

        private MemberViewModelBase _currentUser;
        private DispatcherTimer _timer;
        private ObservableCollection<MemberViewModelBase> _users;
        private ObservableCollection<MemberViewModelBase> _usersBackup;
        private bool _isBackupDoan;

        private ICommand _connect;
        private ICommand _sendMessage;
        private ICommand _blockUser;
        private ICommand _muteUser;
        private ICommand _setBlackListState;
        private ICommand _setMessageToUser;
        private ICommand _typing;
        private ICommand _stopTyping;
        private ICommand _createGroup;

        public ChatViewModel(ChatService chatService, ChatGroupService chatGroupService,
            RegistrationChatService registrationChatService, UserViewModel user, IMapper mapper)
        {
            _mapper = mapper;
            _chatService = chatService;
            _chatGroupService = chatGroupService;
            _registrationChatService = registrationChatService;
            User = user;

            Users = new ObservableCollection<MemberViewModelBase>();
            _blockedUsers = new List<ChatMemberViewModel>();

            _timer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 2)
            };

            SetEvents();
        }

        public UserViewModel User { get; }

        public ObservableCollection<MemberViewModelBase> Users 
        {
            get => _users;
            set 
            {
                _users = value;

                OnPropertyChanged();
            } 
        }
        
        public MemberViewModelBase CurrentUser 
        {
            get => _currentUser;
            set 
            {
                _currentUser = value;

                OnPropertyChanged();
            }
        }

        #region Commands

        public ICommand Connect => _connect ??= new RelayCommandAsync(
            execute: ExecuteConnectEvent);

        private async Task ExecuteConnectEvent(object parametr) 
        {
            await _chatService.Connect();
        }

        public ICommand SendMessage => _sendMessage ??= new RelayCommandAsync(
            execute: ExecuteSendMessage);

        private async Task ExecuteSendMessage(object parametr) 
        {
            if (CurrentUser.EditMessage is null)
            {
                await ExecuteSendMessage();
            }
            else 
            {
                await ExecuteChangeMessage();
            }

            if (User.MessageCreater.FileMessage is not null)
            {
                User.MessageCreater.DoesHideCreater = true;
                User.MessageCreater.DoesHideCreater = false;
                User.MessageCreater.FileMessage = null;
            }

            User.MessageCreater.TextMessage = string.Empty;
        }

        private async Task ExecuteSendMessage() 
        {
            ChatMemberViewModel user = Users.First(user => user.Name == CurrentUser.Name) as ChatMemberViewModel;
            ChatMemberViewModel currentUser = CurrentUser as ChatMemberViewModel;

            ChatMessageModel sendingMessage = await _chatService.ReciveMessageUserAsync(
                connectionId: user.ConnectionId,
                chatMessage: new ChatMessageModel 
                {
                    ChatId = currentUser.ChatId,
                    FromUserId = User.Id,
                    ToUserId = currentUser.Id,
                    Message = User.MessageCreater.TextMessage,
                    ByteFile = User.MessageCreater.FileMessage,
                    SendingTime = DateTime.Now
                });

            sendingMessage.IsFromCurrentUser = true;

            CurrentUser.Messages.AddViewModel(
                item: _mapper.Map<ChatMessageViewModel>(sendingMessage),
                handler: OnMessageChanged);

            CurrentUser.LastMessage = _mapper.Map<ChatMessageViewModel>(sendingMessage);

            Users = Users.GetSortedCollectionByLastMessage();
        }

        private async Task ExecuteChangeMessage() 
        {
            ChatMemberViewModel currentUser = CurrentUser as ChatMemberViewModel;

            await _chatService.SetMessageToChatMessageAsync(
                connectionId: currentUser.ConnectionId,
                userId: User.Id,
                messageId: CurrentUser.EditMessage.Id,
                message: User.MessageCreater.TextMessage);

            CurrentUser.EditMessage.IsEdit = true;
            CurrentUser.EditMessage.Message = User.MessageCreater.TextMessage;
            CurrentUser.EditMessage.IsEditing = false;
        }

        public ICommand BlockUser => _blockUser ??= new RelayCommandAsync(
            execute: ExecuteBlockUser);

        public async Task ExecuteBlockUser(object isBlocked) 
        {
            ChatMemberViewModel currentUser = CurrentUser as ChatMemberViewModel;

            await _chatService.SetBlockStateToUserAsync(
                userId: currentUser.Id,
                connectionId: currentUser.ConnectionId,
                isBlocked: (bool)isBlocked);
        }

        public ICommand MuteUser => _muteUser ??= new RelayCommandAsync(
            execute: ExecuteMuteUser);

        private async Task ExecuteMuteUser(object isMuted) 
        {
            ChatMemberViewModel currentUser = CurrentUser as ChatMemberViewModel;

            await _chatService.SetMuteStateToUserAsync(
                userId: currentUser.Id,
                connectionId: currentUser.ConnectionId,
                isMuted: (bool)isMuted);
        }

        public ICommand SetBlackListState => _setBlackListState ??= new RelayCommandAsync(
            execute: ExecuteSetBlackListState);

        private async Task ExecuteSetBlackListState(object doesBlock) 
        {
            ChatMemberViewModel currentUser = CurrentUser as ChatMemberViewModel;

            await _chatService.SendBlackListStateAsync(
                userId: User.Id,
                connectionId: currentUser.ConnectionId,
                blockedUserId: currentUser.Id,
                doesBlock: (bool)doesBlock);
        }

        public ICommand SetMessageToUser => _setMessageToUser ??= new RelayCommand(
            execute: ExecuteSetMessageToUser);

        private void ExecuteSetMessageToUser(object parametr)
        {
            if (CurrentUser is null)
            {
                return;
            }

            User.MessageCreater.TextMessage = CurrentUser.Draft.Message;
        }

        public ICommand Typing => _typing ??= new RelayCommandAsync(
            execute: ExecuteTyping);

        private async Task ExecuteTyping(object parametr) 
        {
            ChatMemberViewModel currentUser = CurrentUser as ChatMemberViewModel;

            await _chatService.SendUserTypingStatusToUserAsync(
                isTyping: true,
                connectionId: currentUser.ConnectionId,
                typingUserId: User.Id);
            
            if (string.IsNullOrEmpty(User.MessageCreater.TextMessage))
            {
                await _chatService.SendUserTypingStatusToUserAsync(
                    isTyping: false,
                    connectionId: currentUser.ConnectionId,
                    typingUserId: User.Id);
            }
            else
            {
                _timer.Stop();
            }
        }

        public ICommand StopTyping => _stopTyping ??= new RelayCommandAsync(
            execute: ExecuteStopTyping);

        private Task ExecuteStopTyping(object parametr) 
        {
            _timer.Start();

            return Task.CompletedTask;
        }

        public ICommand CreateGroup => _createGroup ??= new RelayCommandAsync(
            execute: ExecuteCreateGroup);

        private async Task ExecuteCreateGroup(object parametr) 
        {
            User.GroupCreater.SetOwnerGroup(new GroupUserViewModel
            {
                Id = User.Id,
                Name = User.Name,
                Photo = User.Photo,
                ConnectionId = User.ConnectionId
            });

            Users.Add(await User.GroupCreater.CreateGroup());

            User.GroupCreater.DoesCloseWindow = true;
            User.GroupCreater.DoesCloseWindow = false;
        }

        #endregion 

        private void SetEvents()
        {
            _chatService.ConnectUser += ConnectUserEventHandler;
            _chatService.Logout += LogoutEventHandler;
            _chatService.ReciveMessage += ReciveMessageEventHandler;
            _chatService.SendConnectionsIdToCallerEvent += SendConnectionsIdToCallerEventHandler;
            _chatService.SetBlockStateUserToAllUsersExeptBlocked += SetBlockStateUserToAllUsersExeptBlockedEventHandler;
            _chatService.SetMuteStateToUser += SetMuteStateToUser;
            _chatService.SetMuteStateUserToAllUsersExeptMuted += SetMuteStateUserToAllUsersExeptMutedEventHandler;
            _chatService.SendBlackListStateToUserServerHandler += SendBlackListStateToUserServerEventHandler;
            _chatService.SendTypingStatusToUserServerHandler += SendTypingStatusToUserServerEventHandler;
            _chatService.SetNewPhotoToUserServerHandler += SetNewPhotoToUserServerEventHandler;
            _chatService.ChangeMessageToUserServerHandler += ChangeMessageToUserServerEventHandler;
            _chatService.SendConnectionIdToCallerServerHandler += OnSendConnectionIdToCallerServerEventHandler;

            _registrationChatService.RegisterUserToOthersServerHandler += RegisterUserToOthersServerEventHandler;
            _registrationChatService.SendUsersToCallerServerHandler += SendUsersToCallerServerEventHandler;
            _registrationChatService.SendUserToCallerServerHandler += SendUserToCallerServerEventHandler;
            _registrationChatService.SendBlockersToCallerServerHandler += SendBlockersToCallerServerEventHandler;

            _chatGroupService.SendGorupToGroupMembersAsyncServerHandler += OnSendGorupToGroupMembersAsyncServerEventHandler;
            _chatGroupService.SendMessageToGroupMembersAsyncServerHandler += OnSendMessageToGroupMembersAsyncServerEventHandler;
            _chatGroupService.SendNewGroupMemberToGroupMembersAsyncServerHandler += OnSendNewGroupMemberToGroupMembersAsyncServerEventHandler;
            _chatGroupService.RemoveGroupMembertToGroupMembersAsyncServerHandler += OnRemoveGroupMembertToGroupMembersAsyncServerEventHandler;

            _timer.Tick += OnTick;

            User.MessageCreater.PropertyChanged += SetMessageToCurrentUserFromUser;
            User.PropertyChanged += SetListOfUsersBySearchingString;
        }

        #region ChatService EventHandlers

        private void ConnectUserEventHandler(string userName, string connectionId) 
        {
            ChatMemberViewModel connectedUser = Users.FirstOrDefault(user =>
            {
                return user.Name == userName;
            }) as ChatMemberViewModel;

            connectedUser.IsLogin = true;
            connectedUser.ConnectionId = connectionId;
        }

        private void LogoutEventHandler(string userName, DateTime disconnectTime) 
        {
            ChatMemberViewModel logoutedUser = Users.FirstOrDefault(user =>
            {
                return user.Name == userName;
            }) as ChatMemberViewModel;

            if (logoutedUser is null)
            {
                logoutedUser = _blockedUsers.Find(user =>
                {
                    return user.Name == userName;
                });
            }

            logoutedUser.ConnectionId = string.Empty;
            logoutedUser.DisconnectTime = disconnectTime;
            logoutedUser.IsLogin = false;
            logoutedUser.IsTyping = false;
        }

        private void ReciveMessageEventHandler(ChatMessageModel message) 
        {
            ChatMemberViewModel userSender = Users.First(user =>
            {
                ChatMemberViewModel member = user as ChatMemberViewModel;

                return member.ChatId.Equals(message.ChatId);
            }) as ChatMemberViewModel;

            userSender.Messages.AddViewModel(
                item: _mapper.Map<ChatMessageViewModel>(message),
                handler: OnMessageChanged);

            userSender.LastMessage = _mapper.Map<ChatMessageViewModel>(message);

            Users = Users.GetSortedCollectionByLastMessage();
        }

        private void SendConnectionsIdToCallerEventHandler(IEnumerable<UserConnection> connections) 
        {
            foreach (UserConnection connection in connections)
            {
                ChatMemberViewModel user = Users.FirstOrDefault(model =>
                {
                    return model.Name == connection.UserName;
                }) as ChatMemberViewModel;

                if (user is null)
                {
                    user = _blockedUsers.Find(user =>
                    {
                        return user.Name == connection.UserName;
                    });
                }

                user.ConnectionId = connection.ConnectionId;
                user.IsLogin = true;
            }
        }

        private void SetBlockStateUserToAllUsersExeptBlockedEventHandler(string userId, bool isBlocked) 
        {
            ChatMemberViewModel chatMember = Users.FirstOrDefault(user =>
            {
                ChatMemberViewModel member = user as ChatMemberViewModel;

                return member.Id == userId;
            }) as ChatMemberViewModel;

            if (chatMember is null)
            {
                chatMember = _blockedUsers.Find(user =>
                {
                    return user.Id == userId;
                });
            }

            chatMember.IsBlocked = isBlocked;

            if (User.IsAdmin)
            {
                return;
            }

            if (isBlocked)
            {
                _blockedUsers.Add(chatMember);
                Users.Remove(chatMember);
            }
            else 
            {
                Users.Add(chatMember);
                _blockedUsers.Remove(chatMember);

                Users = Users.GetSortedCollectionByLastMessage();
            }
        }

        private void SetMuteStateToUser(bool isMuted) 
        {
            User.IsMuted = isMuted;

            User.MessageCreater.TextMessage = string.Empty;

            foreach (ChatMemberViewModel user in Users)
            {
                user.Draft.Message = null;
            }
        }

        private void SetMuteStateUserToAllUsersExeptMutedEventHandler(string userId, bool isMuted) 
        {
            ChatMemberViewModel member = Users.First(user =>
            {
                ChatMemberViewModel member = user as ChatMemberViewModel;

                return member.Id == userId;
            }) as ChatMemberViewModel;

            if (member is null)
            {
                member = _blockedUsers.First(user =>
                {
                    return user.Id == userId;
                });
            }

            member.IsMuted = isMuted; 
        }

        private void SendBlackListStateToUserServerEventHandler(BlockModel block) 
        {
            ChatMemberViewModel member = Users.First(user =>
            {
                ChatMemberViewModel member = user as ChatMemberViewModel;

                return member.Id == block.UserId;
            }) as ChatMemberViewModel;

            member.IsClientBlockedByMember = block.DoesBlocked;
            member.Draft.Message = null;

            if (CurrentUser is not null && CurrentUser.Equals(member))
            {
                User.MessageCreater.TextMessage = string.Empty;
            }
        }

        private void SendTypingStatusToUserServerEventHandler(bool isTyping, string typingUserId) 
        {
            ChatMemberViewModel user = Users.First(userModel =>
            {
                ChatMemberViewModel member = userModel as ChatMemberViewModel;

                return member.Id == typingUserId;
            }) as ChatMemberViewModel;

            user.IsTyping = isTyping;
        }

        private void SetNewPhotoToUserServerEventHandler(string userName, byte[] photo) 
        {
            ChatMemberViewModel user = Users.First(userModel =>
            {
                return userModel.Name == userName;
            }) as ChatMemberViewModel;

            user.Photo = photo;
        }

        private void ChangeMessageToUserServerEventHandler(string userId, Guid messageId, string message) 
        {
            ChatMemberViewModel user = Users.First(userModel =>
            {
                ChatMemberViewModel member = userModel as ChatMemberViewModel;

                return member.Id == userId;
            }) as ChatMemberViewModel;

            ChatMessageViewModel chatMessage = user.Messages.First(messageModel =>
            {
                return messageModel.Id == messageId;
            }) as ChatMessageViewModel;

            chatMessage.Message = message;
            chatMessage.IsEdit = true;
        }

        private void OnSendConnectionIdToCallerServerEventHandler(string connectionId) 
        {
            User.ConnectionId = connectionId;
        }

        #endregion

        #region RegistrationChatService EventHandlers

        private void RegisterUserToOthersServerEventHandler(FullUserModel newUser)
        {
            ChatModel chat = newUser.Chats.Find(chatModel =>
            {
                return chatModel.FirstUserId.Equals(User.Id) || chatModel.SecondUserId.Equals(User.Id);
            });

            ChatMemberViewModel user = _mapper.Map<ChatMemberViewModel>(newUser);

            user.ChatId = chat.Id;
            user.Messages = _mapper.Map<ObservableCollection<MessageViewModelBase>>(chat.Messages);
            user.LastMessage = chat.Messages.Count == 0
                    ? new ChatMessageViewModel()
                    : _mapper.Map<ChatMessageViewModel>(chat.Messages.Last());

            user.Messages.SetPropertyChangedEventHandler(OnMessageChanged);

            if (_isBackupDoan)
            {
                _usersBackup.Add(user);
            }
            else 
            {
                Users.Add(user);
            }
        }

        private void SendUsersToCallerServerEventHandler(IEnumerable<UserModel> users) 
        {
            foreach (UserModel user in users)
            {
                ChatMemberViewModel member = _mapper.Map<ChatMemberViewModel>(user);

                member.Messages.SetPropertyChangedEventHandler(OnMessageChanged);

                if (user.IsBlocked && !User.IsAdmin)
                {
                    _blockedUsers.Add(member);
                }
                else
                {
                    Users.Add(member);
                }
            }

            Users = Users.GetSortedCollectionByLastMessage();
        }

        private void SendUserToCallerServerEventHandler(FullUserModel user) 
        {
            User.Id = user.Id;
            User.Name = user.Name;
            User.IsAdmin = user.IsAdmin;
            User.IsMuted = user.IsMuted;
            User.Photo = user.Photo;

            foreach (GroupModel group in user.Groups)
            {
                GroupViewModel groupViewModel = _mapper.Map<GroupViewModel>(group);

                groupViewModel.LastMessage = groupViewModel.Messages is null || groupViewModel.Messages.Count == 0
                    ? new GroupMessageViewModel()
                    : groupViewModel.Messages.Last();

                Users.Add(groupViewModel);
            }
        }

        private void SendBlockersToCallerServerEventHandler(IEnumerable<BlockModel> blocks) 
        {
            foreach (BlockModel block in blocks) 
            {
                ChatMemberViewModel member = Users.FirstOrDefault(user =>
                {
                    ChatMemberViewModel member = user as ChatMemberViewModel;

                    return member.Id == block.UserId;
                }) as ChatMemberViewModel;

                member.IsClientBlockedByMember = block.DoesBlocked;
            }
        }

        #endregion

        #region ChatGroupService EventHandlers

        private void OnSendGorupToGroupMembersAsyncServerEventHandler(GroupModel group) 
        {
            Users.Add(_mapper.Map<GroupViewModel>(group));
        }

        private void OnSendMessageToGroupMembersAsyncServerEventHandler(GroupMessageModel messageModel) 
        {
            GroupViewModel group = GetGroupViewModelById(messageModel.GroupId);

            GroupMessageViewModel message = _mapper.Map<GroupMessageViewModel>(messageModel);

            message.IsFromCurrentUser = User.Id == messageModel.SenderId;

            group.Messages.Add(message);
        }

        private void OnSendNewGroupMemberToGroupMembersAsyncServerEventHandler(GroupUser user, string groupName) 
        {
            GroupViewModel group = GetGroupViewModelByName(groupName);

            group.Users.Add(_mapper.Map<GroupUserViewModel>(user));
        }

        private void OnRemoveGroupMembertToGroupMembersAsyncServerEventHandler(GroupUser user, string groupName) 
        {
            GroupViewModel group = GetGroupViewModelByName(groupName);

            group.Users.Remove(group.Users.First(userInGroup => userInGroup.Id == user.Id));
        }

        #endregion

        private async void OnTick(object sender, EventArgs e) 
        {
            ChatMemberViewModel currentUser = CurrentUser as ChatMemberViewModel;

            await _chatService.SendUserTypingStatusToUserAsync(
                isTyping: false,
                connectionId: currentUser.ConnectionId,
                typingUserId: User.Id);

            _timer.Stop();
        }

        #region OnPropertyChanged Handlers

        private void SetMessageToCurrentUserFromUser(object sender, PropertyChangedEventArgs e) 
        {
            if (CurrentUser is null || e.PropertyName != nameof(UserViewModel.MessageCreater.TextMessage))
            {
                return;
            }

            MessageCreaterViewModel messageCreater = sender as MessageCreaterViewModel;

            if (string.IsNullOrEmpty(messageCreater.TextMessage))
            {
                CurrentUser.Draft.Message = null;
            }
            else
            {
                if (CurrentUser.Draft.Message is null)
                {
                    CurrentUser.Draft.StartTypingTime = DateTime.Now;
                }

                CurrentUser.Draft.Message = messageCreater.TextMessage;
            }
        }

        private void SetListOfUsersBySearchingString(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(UserViewModel.SearchingUser))
            {
                return;
            }

            UserViewModel user = sender as UserViewModel;

            if (string.IsNullOrEmpty(user.SearchingUser) || string.IsNullOrWhiteSpace(user.SearchingUser))
            {
                Users = _usersBackup;

                _isBackupDoan = false;
            }
            else 
            {
                if (!_isBackupDoan)
                {
                    _usersBackup = Users;

                    _isBackupDoan = true;
                }

                Users = new ObservableCollection<MemberViewModelBase>(_usersBackup.Where(userModel => userModel.Name.Contains(user.SearchingUser)));
            }
        }

        private void OnMessageChanged(object sender, PropertyChangedEventArgs e) 
        {
            if (CurrentUser is null || e.PropertyName != nameof(ChatMessageViewModel.IsEditing))
            {
                return;
            }

            ChatMessageViewModel message = sender as ChatMessageViewModel;

            if (message.IsEditing)
            {
                CurrentUser.EditMessage = message;

                User.MessageCreater.TextMessage = message.Message;
            }
            else 
            {
                CurrentUser.EditMessage = null;
            }
        }

        #endregion

        private GroupViewModel GetGroupViewModelByName(string groupName) =>
            Users.FirstOrDefault(member =>
            {
                GroupViewModel group = member as GroupViewModel;

                if (group is null)
                {
                    return false;
                }

                return group.Name == groupName;
            }) as GroupViewModel;

        private GroupViewModel GetGroupViewModelById(Guid id) =>
            Users.FirstOrDefault(member =>
            {
                GroupViewModel group = member as GroupViewModel;

                if (group is null)
                {
                    return false;
                }

                return group.Id == id;
            }) as GroupViewModel;
    }
}
