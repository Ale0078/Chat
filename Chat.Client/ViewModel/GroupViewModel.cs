using System;
using System.Collections.ObjectModel;

using Chat.Client.ViewModel.Base;

namespace Chat.Client.ViewModel
{
    public class GroupViewModel : MemberViewModelBase
    {
        private Guid _id;

        public GroupViewModel() : base()
        {
            IsGroup = true;

            Users = new ObservableCollection<GroupUserViewModel>();
        }

        public ObservableCollection<GroupUserViewModel> Users { get; set; }

        public Guid Id 
        {
            get => _id;
            set 
            {
                _id = value;

                OnPropertyChanged();
            }
        }
    }
}
