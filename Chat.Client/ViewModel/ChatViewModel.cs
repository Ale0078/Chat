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
using Chat.Client.Services.Interfaces;

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
        private ChatMemberViewModel _userToSendTypingStatus;
        private DispatcherTimer _timer;
        private ObservableCollection<MemberViewModelBase> _users;
        private ObservableCollection<MemberViewModelBase> _usersBackup;
        private bool _isBackupDoan;
        private MemberViewModelBase _bufferReferenceToCurrentUser;

        private ICommand _connect;
        private ICommand _sendMessage;
        private ICommand _blockUser;
        private ICommand _muteUser;
        private ICommand _setBlackListState;
        private ICommand _setMessageToUser;
        private ICommand _typing;
        private ICommand _stopTyping;
        private ICommand _createGroup;
        private ICommand _loadFirstChosenUser;

        public ChatViewModel(ChatService chatService, ChatGroupService chatGroupService,
            RegistrationChatService registrationChatService, UserViewModel user, 
            IMapper mapper, IScrollController scrollController)
        {
            _mapper = mapper;
            _chatService = chatService;
            _chatGroupService = chatGroupService;
            _registrationChatService = registrationChatService;

            User = user;
            ScrollController = scrollController;

            Users = new ObservableCollection<MemberViewModelBase>();
            _blockedUsers = new List<ChatMemberViewModel>();

            _timer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 2)
            };

            SetEvents();
        }

        public UserViewModel User { get; }

        public IScrollController ScrollController { get; }

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
                if (_currentUser is not null)
                {
                    _currentUser.LastVerticalOffsetToMessages = ScrollController.GetVerticalOffset();
                }

                if (_currentUser is not null && !_currentUser.WasSelected)
                {
                    _currentUser.WasSelected = true;
                }

                _currentUser = value;

                ScrollController.SetVerticalOffset(0);

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
                if (CurrentUser is GroupViewModel)
                {
                    await ExecuteSendGroupMessage();
                }
                else
                {
                    await ExecuteSendMessage();
                }
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

            ScrollController.ScrollToEnd();
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

            ChatMessageViewModel message = _mapper.Map<ChatMessageViewModel>(sendingMessage);

            CurrentUser.Messages.AddViewModel(
                item: message,
                OnMessageChanged, OnMessageIsReadChanged);

            CurrentUser.LastMessage = message;

            Users = Users.GetSortedCollectionByLastMessage();
        }

        private async Task ExecuteSendGroupMessage() 
        {
            GroupViewModel group = CurrentUser as GroupViewModel;

            GroupMessageModel messageModel = await _chatGroupService.SendGroupMessageAsync(
                groupName: CurrentUser.Name,
                message: new GroupMessageModel
                {
                    GroupId = group.Id,
                    SenderId = User.Id,
                    FileMessage = User.MessageCreater.FileMessage,
                    TextMessage = User.MessageCreater.TextMessage,
                    SendingTime = DateTime.Now,
                    IsEdit = false
                });

            GroupMessageViewModel sendingMessage = _mapper.Map<GroupMessageViewModel>(messageModel);

            sendingMessage.IsFromCurrentUser = true;

            CurrentUser.Messages.AddViewModel(
                item: sendingMessage,
                OnMessageChanged, OnMessageIsReadChanged);

            if (CurrentUser.LastMessage.FromUserId == sendingMessage.FromUserId)
            {
                ((GroupMessageViewModel)CurrentUser.LastMessage).IsItLastMessageFromSender = false;
            }

            sendingMessage.IsItLastMessageFromSender = true;

            CurrentUser.LastMessage = sendingMessage;

            ((GroupMessageViewModel)CurrentUser.LastMessage).LastMessageSender = GetLastMessageSenderToGroup(group);

            Users = Users.GetSortedCollectionByLastMessage();
        }

        private async Task ExecuteChangeMessage() 
        {

            if (CurrentUser is ChatMemberViewModel currentUser)
            {
                await _chatService.SetMessageToChatMessageAsync(
                    connectionId: currentUser.ConnectionId,
                    userId: User.Id,
                    messageId: CurrentUser.EditMessage.Id,
                    message: User.MessageCreater.TextMessage);
            }
            else 
            {
                await _chatGroupService.ChangeMessageAsync(
                    groupName: CurrentUser.Name,
                    messageId: CurrentUser.EditMessage.Id,
                    message: User.MessageCreater.TextMessage);
            }

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

        public ICommand SetMessageToUser => _setMessageToUser ??= new RelayCommandAsync(
            execute: ExecuteSetMessageToUser);

        private async Task ExecuteSetMessageToUser(object parametr)
        {
            if (CurrentUser is null)
            {
                return;
            }

            User.MessageCreater.TextMessage = CurrentUser.Draft.Message;

            if (_userToSendTypingStatus is null 
                || string.IsNullOrEmpty(_userToSendTypingStatus.ConnectionId))
            {
                return;
            }

            _timer.Stop();

            await _chatService.SendUserTypingStatusToUserAsync(
                isTyping: false,
                connectionId: _userToSendTypingStatus.ConnectionId,
                typingUserId: User.Id);
        }

        public ICommand Typing => _typing ??= new RelayCommandAsync(
            execute: ExecuteTyping);

        private async Task ExecuteTyping(object parametr) 
        {
            ChatMemberViewModel currentUser = CurrentUser as ChatMemberViewModel;

            if (currentUser is null) // ToDo: Remove IT!!!
            {
                return;
            }

            if (string.IsNullOrEmpty(currentUser.ConnectionId))
            {
                return;
            }

            _userToSendTypingStatus = currentUser;

            await _chatService.SendUserTypingStatusToUserAsync(
                isTyping: true,
                connectionId: currentUser.ConnectionId,
                typingUserId: User.Id);
            
            if (string.IsNullOrEmpty(User.MessageCreater.TextMessage)
                || User.MessageCreater.IsPlaceholedApplied)
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
            if (CurrentUser is GroupViewModel)//ToDo: Remove IT!!!!
            {
                return Task.CompletedTask;
            }

            if (string.IsNullOrEmpty(((ChatMemberViewModel)CurrentUser).ConnectionId))
            {
                return Task.CompletedTask;
            }

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
                User = User
            });

            Users.Add(await User.GroupCreater.CreateGroup());

            User.GroupCreater.DoesCloseWindow = true;
            User.GroupCreater.DoesCloseWindow = false;
        }

        public ICommand LoadFirstChosenUser => _loadFirstChosenUser ??= new RelayCommand(
            execute: ExecuteLoadFirstChosenUser);

        private void ExecuteLoadFirstChosenUser(object parametr) 
        {
            if (CurrentUser.Equals(_bufferReferenceToCurrentUser))
            {
                return;
            }

            if (!_currentUser.WasSelected)
            {
                ScrollMessages();
            }
            else if (_currentUser.LastVerticalOffsetToMessages is not double.NaN)
            {
                ScrollController.SetVerticalOffset(CurrentUser.LastVerticalOffsetToMessages);
            }

            _bufferReferenceToCurrentUser = CurrentUser;
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
            _chatService.SetReadStatusToMessageToUserServerHandler += OnSetReadStatusToMessageToUserServerEventHandler;

            _registrationChatService.RegisterUserToOthersServerHandler += RegisterUserToOthersServerEventHandler;
            _registrationChatService.SendUsersToCallerServerHandler += SendUsersToCallerServerEventHandler;
            _registrationChatService.SendUserToCallerServerHandler += SendUserToCallerServerEventHandler;
            _registrationChatService.SendBlockersToCallerServerHandler += SendBlockersToCallerServerEventHandler;

            _chatGroupService.SendGorupToGroupMembersAsyncServerHandler += OnSendGorupToGroupMembersAsyncServerEventHandler;
            _chatGroupService.SendMessageToGroupMembersAsyncServerHandler += OnSendMessageToGroupMembersAsyncServerEventHandler;
            _chatGroupService.SendNewGroupMemberToGroupMembersAsyncServerHandler += OnSendNewGroupMemberToGroupMembersAsyncServerEventHandler;
            _chatGroupService.RemoveGroupMembertToGroupMembersAsyncServerHandler += OnRemoveGroupMembertToGroupMembersAsyncServerEventHandler;
            _chatGroupService.ChangedGrouMessageToGroupMembersAsyncServerHanler += ChangedGrouMessageToGroupMembersAsyncServerEventHanler;

            _timer.Tick += OnTick;

            User.MessageCreater.PropertyChanged += SetMessageToCurrentUserFromUser;
            User.PropertyChanged += SetListOfUsersBySearchingString;
        }

        #region ChatService EventHandlers

        private void ConnectUserEventHandler(string userName, string connectionId) 
        {
            ChatMemberViewModel connectedUser = Users.FirstOrDefault(user =>
            {
                if (user is GroupViewModel)
                {
                    return false;
                }

                return user.Name == userName;
            }) as ChatMemberViewModel;

            connectedUser.IsLogin = true;
            connectedUser.ConnectionId = connectionId;
        }

        private void LogoutEventHandler(string userName, DateTime disconnectTime) 
        {
            ChatMemberViewModel logoutedUser = Users.FirstOrDefault(user =>
            {
                if (user is GroupViewModel)
                {
                    return false;
                }

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

                if (member is null)
                {
                    return false;
                }

                return member.ChatId.Equals(message.ChatId);
            }) as ChatMemberViewModel;

            ChatMessageViewModel chatMessage = _mapper.Map<ChatMessageViewModel>(message);

            userSender.Messages.AddViewModel(
                item: chatMessage,
                OnMessageChanged, OnMessageIsReadChanged);

            userSender.LastMessage = chatMessage;

            Users = Users.GetSortedCollectionByLastMessage();

            ScrollToEndIfStentInTheEnd();
        }

        private void SendConnectionsIdToCallerEventHandler(IEnumerable<UserConnection> connections) 
        {
            foreach (UserConnection connection in connections)
            {
                ChatMemberViewModel user = Users.FirstOrDefault(model =>
                {
                    if (model is GroupViewModel)
                    {
                        return false;
                    }

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

            foreach (MemberViewModelBase user in Users)
            {
                user.Draft.Message = null;
            }
        }

        private void SetMuteStateUserToAllUsersExeptMutedEventHandler(string userId, bool isMuted) 
        {
            ChatMemberViewModel member = Users.First(user =>
            {
                ChatMemberViewModel member = user as ChatMemberViewModel;

                if (member is null)
                {
                    return false;
                }

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

                if (member is null)
                {
                    return false;
                }

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

                if (member is null)
                {
                    return false;
                }

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

        private void OnSetReadStatusToMessageToUserServerEventHandler(string userId, Guid messageId) 
        {
            ChatMemberViewModel user = Users.First(userModel =>
            {
                ChatMemberViewModel member = userModel as ChatMemberViewModel;

                if (member is null)
                {
                    return false;
                }

                return member.Id == userId;
            }) as ChatMemberViewModel;

            MessageViewModelBase message = user.Messages.First(message =>
            {
                return message.Id == messageId;
            });

            message.IsRead = true;
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
                    : user.Messages.Last();

            user.Messages.SetPropertyChangedEventHandler(OnMessageChanged);
            user.Messages.SetPropertyChangedEventHandler(OnMessageIsReadChanged);

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
                member.Messages.SetPropertyChangedEventHandler(OnMessageIsReadChanged);

                foreach (MemberViewModelBase memberGroup in Users)
                {
                    if (memberGroup is GroupViewModel group)
                    {
                        foreach (GroupUserViewModel groupUser in group.Users)
                        {
                            if (groupUser.Id == member.Id)
                            {
                                groupUser.User = member;
                            }
                            else if (groupUser.Id == User.Id) 
                            {
                                groupUser.User = User;
                            }
                        }
                    }
                }

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
                GroupViewModel groupViewModel = new GroupViewModel
                {
                    Id = group.Id,
                    Name = group.Name,
                    Photo = group.Photo,
                    Users = _mapper.Map<ObservableCollection<GroupUserViewModel>>(group.Users)
                };

                GroupMessageViewModel buffer = new();

                foreach (GroupMessageModel message in group.GroupMessages)
                {
                    GroupMessageViewModel groupMessage = _mapper.Map<GroupMessageViewModel>(message);

                    groupMessage.IsFromCurrentUser = message.SenderId == User.Id;

                    if (buffer.FromUserId == groupMessage.FromUserId)
                    {
                        buffer.IsItLastMessageFromSender = false;
                    }

                    groupMessage.IsItLastMessageFromSender = true;
                    groupMessage.LastMessageSender = GetGroupUser(groupViewModel, groupMessage.FromUserId);

                    groupViewModel.Messages.AddViewModel(
                        item: groupMessage,
                        OnMessageChanged, OnMessageIsReadChanged);

                    buffer = groupMessage;
                }

                if (groupViewModel.Messages is null || groupViewModel.Messages.Count == 0)
                {
                    groupViewModel.LastMessage = new GroupMessageViewModel();
                }
                else 
                {
                    MessageViewModelBase newLastMessage = groupViewModel.Messages.Last();

                    groupViewModel.LastMessage = newLastMessage;

                    ((GroupMessageViewModel)groupViewModel.LastMessage).LastMessageSender = GetLastMessageSenderToGroup(groupViewModel);
                }

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

                    if (member is null)
                    {
                        return false;
                    }

                    return member.Id == block.UserId;
                }) as ChatMemberViewModel;

                member.IsClientBlockedByMember = block.DoesBlocked;
            }
        }

        #endregion

        #region ChatGroupService EventHandlers

        private void OnSendGorupToGroupMembersAsyncServerEventHandler(GroupModel group) 
        {
            GroupViewModel newGroup = _mapper.Map<GroupViewModel>(group);

            newGroup.LastMessage = new GroupMessageViewModel();

            foreach (MemberViewModelBase member in Users)
            {
                if (member is ChatMemberViewModel user)
                {
                    foreach (GroupUserViewModel groupUser in newGroup.Users)
                    {
                        if (groupUser.Id == user.Id)
                        {
                            groupUser.User = user;
                        }
                        else if (groupUser.Id == User.Id)
                        {
                            groupUser.User = User;
                        }
                    }
                }
            }

            Users.Add(newGroup);
        }

        private void OnSendMessageToGroupMembersAsyncServerEventHandler(GroupMessageModel messageModel) 
        {
            GroupViewModel group = GetGroupViewModelById(messageModel.GroupId);

            GroupMessageViewModel message = _mapper.Map<GroupMessageViewModel>(messageModel);

            message.IsFromCurrentUser = User.Id == messageModel.SenderId;

            group.Messages.Add(message);

            if (group.LastMessage.FromUserId == message.FromUserId)
            {
                ((GroupMessageViewModel)group.LastMessage).IsItLastMessageFromSender = false;
            }

            message.IsItLastMessageFromSender = true;

            group.LastMessage = message;

            ((GroupMessageViewModel)group.LastMessage).LastMessageSender = GetLastMessageSenderToGroup(group);

            Users = Users.GetSortedCollectionByLastMessage();

            ScrollToEndIfStentInTheEnd();
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

        private void ChangedGrouMessageToGroupMembersAsyncServerEventHanler(string groupName, Guid messageId, string message) 
        {
            GroupViewModel group = GetGroupViewModelByName(groupName);

            GroupMessageViewModel groupMessage = group.Messages.First(groupMessage =>
            {
                return groupMessage.Id == messageId;
            }) as GroupMessageViewModel;

            groupMessage.Message = message;
            groupMessage.IsEdit = true;
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
            if (CurrentUser is null || e.PropertyName != nameof(MessageViewModelBase.IsEditing))
            {
                return;
            }

            MessageViewModelBase message = sender as MessageViewModelBase;

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

        private async void OnMessageIsReadChanged(object sender, PropertyChangedEventArgs e) 
        {
            if (CurrentUser is null || e.PropertyName != nameof(MessageViewModelBase.IsRead))
            {
                return;
            }

            MessageViewModelBase message = sender as MessageViewModelBase;
            
            if (!message.IsRead || message.IsFromCurrentUser)
            {
                return;
            }

            if (CurrentUser.IsGroup)
            {
                //throw new NotImplementedException();
            }
            else 
            {
                ChatMemberViewModel user = CurrentUser as ChatMemberViewModel;

                await _chatService.ReadMessageToUserAsync(
                    userIdToSendReadStatus: User.Id,
                    connectionId: user.ConnectionId,
                    messageId: message.Id);
            }
        }

        #endregion

        #region Halper Functions

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

        private GroupUserViewModel GetLastMessageSenderToGroup(GroupViewModel group) 
        {
            foreach (GroupUserViewModel groupUser in group.Users)
            {
                if (groupUser.Id == group.LastMessage.FromUserId)
                {
                    return groupUser;
                }
            }

            return null;
        }

        private GroupUserViewModel GetGroupUser(GroupViewModel group, string userId) 
        {
            foreach (GroupUserViewModel user in group.Users)
            {
                if (user.Id == userId)
                {
                    return user;
                }
            }

            return null;
        }

        private void ScrollMessages()
        {
            if (CurrentUser.CountUnreadMessages != 0)
            {
                ScrollController.ScrollToFirstUnreadMessage();
            }
            else
            {
                ScrollController.ScrollToEnd();
            }
        }

        private void ScrollToEndIfStentInTheEnd() 
        {
            if (ScrollController.IsScrollToEnd())
            {
                ScrollController.ScrollToEnd();
            }
        }

        #endregion
    }
}
