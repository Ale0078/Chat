using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Chat.Client.Commands;
using Chat.Client.Datas;

namespace Chat.Client.ViewModel
{
    public class GroupCreaterViewModel : ViewModelBase
    {
        private readonly ReferenceByteFile _defaulPhoto;

        private string _groupName;
        private ReferenceByteFile _groupPhoto;
        private bool _doesCloseWindow;

        private ICommand _closeWidow;
        private ICommand _addUserToGroupMembers;

        public GroupCreaterViewModel()
        {
            _defaulPhoto = new ReferenceByteFile
            {
                ByteFile = File.ReadAllBytes("../../../Images/GroupIconImage.png")
            };

            GroupPhoto = _defaulPhoto;

            GroupMembers = new ObservableCollection<ChatMemberViewModel>();
        }

        public ObservableCollection<ChatMemberViewModel> GroupMembers { get; set; }

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

            if (user.IsSelectedToAddToNewGruop)
            {
                user.IsSelectedToAddToNewGruop = false;

                GroupMembers.Remove(user);
            }
            else 
            {
                user.IsSelectedToAddToNewGruop = true;

                GroupMembers.Add(user);
            }
        }
    }
}
