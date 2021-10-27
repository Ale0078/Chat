using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;

using Chat.Client.Commands;
using Chat.Client.Datas;
using Chat.Client.ViewModel.Base;
using Chat.Client.Services;
using Chat.Models;

namespace Chat.Client.ViewModel
{
    public class GroupCreaterViewModel : ViewModelBase
    {
        private readonly IMapper _mapper;
        private readonly ChatGroupService _groupService;
        private readonly ReferenceByteFile _defaulPhoto;

        private string _groupName;
        private ReferenceByteFile _groupPhoto;
        private bool _doesCloseWindow;

        private ICommand _closeWidow;
        private ICommand _addUserToGroupMembers;

        public GroupCreaterViewModel(IMapper mapper, ChatGroupService groupService)
        {
            _mapper = mapper;
            _groupService = groupService;

            _defaulPhoto = new ReferenceByteFile
            {
                ByteFile = File.ReadAllBytes("../../../Images/GroupIconImage.png")
            };

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

        public ReferenceByteFile GroupPhoto 
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
            GroupMembers.Clear();
            GroupName = string.Empty;
            GroupPhoto = _defaulPhoto;

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
                //user.IsSelectedToAddToNewGruop = false;

                GroupMembers.Remove(chatMember as GroupUserViewModel);
            }
            else 
            {
                //user.IsSelectedToAddToNewGruop = true;

                GroupMembers.Add(_mapper.Map<GroupUserViewModel>(user));
            }
        }

        public void SetOwnerGroup(GroupUserViewModel owner) 
        {
            GroupMembers.Add(owner);
        }

        public async Task<GroupViewModel> CreateGroup()
        {
            return _mapper.Map<GroupViewModel>(await _groupService.CreateNewGroupAsync(
                groupName: GroupName,
                groupPhoto: GroupPhoto.ByteFile,
                users: _mapper.Map<List<GroupUser>>(GroupMembers.ToList())));
        }
    }
}
