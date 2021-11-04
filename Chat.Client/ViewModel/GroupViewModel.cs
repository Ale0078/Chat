using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

using Chat.Client.ViewModel.Base;
using Chat.Client.Extensions;

namespace Chat.Client.ViewModel
{
    public class GroupViewModel : MemberViewModelBase
    {
        private Guid _id;
        private int _userOnlineCounter;
        private ObservableCollection<GroupUserViewModel> _users;

        public GroupViewModel() : base()
        {
            _userOnlineCounter = 1;

            IsGroup = true;

            _users = new ObservableCollection<GroupUserViewModel>();
        }

        public Guid Id 
        {
            get => _id;
            set 
            {
                _id = value;

                OnPropertyChanged();
            }
        }

        public int UserOnlineCounter 
        {
            get => _userOnlineCounter;
            set 
            {
                _userOnlineCounter = value;

                OnPropertyChanged();
            }
        }

        public ObservableCollection<GroupUserViewModel> Users 
        {
            get => _users;
            set 
            {
                _users.CollectionChanged -= OnGroupUserAddOrRemove;

                foreach (GroupUserViewModel groupUser in _users) 
                {
                    if (groupUser.User is not null)
                    {
                        groupUser.User.PropertyChanged -= OnUserOnlineStatusChanged;
                    }
                }

                _users.UnsubsribePropertyChangedEventHandler(OnUserChangedToGroupUser);

                _users = value;

                _users.CollectionChanged += OnGroupUserAddOrRemove;
                _users.SetPropertyChangedEventHandler(OnUserChangedToGroupUser);

                OnPropertyChanged();
            }
        }

        private void OnUserOnlineStatusChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ChatMemberViewModel.IsLogin))
            {
                return;
            }

            if (((ChatMemberViewModel)sender).IsLogin)
            {
                UserOnlineCounter++;
            }
            else 
            {
                UserOnlineCounter--;
            }
        }

        private void OnUserChangedToGroupUser(object sender, PropertyChangedEventArgs e) 
        {
            if (e.PropertyName != nameof(GroupUserViewModel.User))
            {
                return;
            }

            GroupUserViewModel groupUser = sender as GroupUserViewModel;

            if (groupUser.User is ChatMemberViewModel chatMember && chatMember.IsLogin)
            {
                UserOnlineCounter++;
            }

            groupUser.User.PropertyChanged += OnUserOnlineStatusChanged;
        }

        private void OnGroupUserAddOrRemove(object sender, NotifyCollectionChangedEventArgs e) 
        {
            throw new NotImplementedException();
        }
    }
}
