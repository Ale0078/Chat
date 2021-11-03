using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;

using Chat.Client.Commands;
using Chat.Client.ViewModel.Base;
using Chat.Client.Services;
using Chat.Client.Services.Interfaces;
using Chat.Models;

namespace Chat.Client.ViewModel
{
    public class GroupCreaterViewModel : ViewModelBase
    {
        private readonly IMapper _mapper;
        private readonly ChatGroupService _groupService;
        private readonly byte[] _defaulPhoto;
        private readonly IDialogService _dialogService;

        private string _groupName;
        private byte[] _groupPhoto;
        private bool _doesCloseWindow;

        private ICommand _closeWidow;
        private ICommand _addUserToGroupMembers;
        private ICommand _setPhoto;

        public GroupCreaterViewModel(IMapper mapper, ChatGroupService groupService, IDialogService dialogService)
        {
            _mapper = mapper;
            _groupService = groupService;
            _dialogService = dialogService;

            _defaulPhoto = File.ReadAllBytes("../../../Images/GroupIconImage.png");

            GroupPhoto = _defaulPhoto;

            GroupMembers = new ObservableCollection<GroupUserViewModel>();
        }

        public ObservableCollection<GroupUserViewModel> GroupMembers { get; set; }

        public string GroupName 
        {
            get => _groupName;
            set 
            {
                _groupName = value;

                OnPropertyChanged();
            }
        }

        public byte[] GroupPhoto 
        {
            get => _groupPhoto;
            set 
            {
                _groupPhoto = value;

                OnPropertyChanged();
            }
        }

        public bool DoesCloseWindow 
        {
            get => _doesCloseWindow;
            set 
            {
                _doesCloseWindow = value;

                OnPropertyChanged();
            }
        }

        public ICommand CloseWindow => _closeWidow ??= new RelayCommand(
            execute: ExecuteCloseWindow);

        private void ExecuteCloseWindow(object parametr) 
        {
            ClearGroupCreater();

            DoesCloseWindow = true;
            DoesCloseWindow = false;
        }

        public ICommand AddUserToGroupMembers => _addUserToGroupMembers ??= new RelayCommand(
            execute: ExecuteAddUserToGroupMembers);

        private void ExecuteAddUserToGroupMembers(object chatMember) 
        {
            ChatMemberViewModel user = chatMember as ChatMemberViewModel;

            if (user is null)
            {
                GroupMembers.Remove(chatMember as GroupUserViewModel);
            }
            else 
            {
                GroupUserViewModel groupUser = _mapper.Map<GroupUserViewModel>(user);

                groupUser.User = user;

                GroupMembers.Add(groupUser);
            }
        }

        public ICommand SetPhoto => _setPhoto ??= new RelayCommandAsync(
            execute: ExecuteSetPhoto);

        private async Task ExecuteSetPhoto(object parametr) 
        {
            string source = _dialogService.OpenFile("Choose image", "Images (*.jpg;*png)|*.jpg;*png");

            if (string.IsNullOrEmpty(source))
            {
                return;
            }

            GroupPhoto = await File.ReadAllBytesAsync(source);
        }

        public void SetOwnerGroup(GroupUserViewModel owner) 
        {
            GroupMembers.Add(owner);
        }

        public async Task<GroupViewModel> CreateGroup()
        {
            try
            {
                GroupViewModel newGroup = _mapper.Map<GroupViewModel>(await _groupService.CreateNewGroupAsync(
                groupName: GroupName,
                groupPhoto: GroupPhoto,
                users: _mapper.Map<List<GroupUser>>(GroupMembers.ToList())));

                newGroup.LastMessage = new GroupMessageViewModel();

                for (int userIndex = 0; userIndex < newGroup.Users.Count; userIndex++)
                {
                    newGroup.Users[userIndex].User = GroupMembers[userIndex].User;
                }

                return newGroup;
            }
            finally 
            {
                ClearGroupCreater();
            }
        }

        private void ClearGroupCreater() 
        {
            GroupMembers.Clear();
            GroupName = string.Empty;
            GroupPhoto = _defaulPhoto;
        }
    }
}
