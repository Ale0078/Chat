using Chat.Client.ViewModel.Base;

namespace Chat.Client.ViewModel
{
    public class GroupUserViewModel : ViewModelBase
    {
        private string _id;
        private UserViewModelBase _user;



        public string Id
        {
            get => _id;
            set
            {
                _id = value;

                OnPropertyChanged();
            }
        }

        public UserViewModelBase User 
        {
            get => _user;
            set 
            {
                _user = value;

                OnPropertyChanged();
            }
        }
    }
}
