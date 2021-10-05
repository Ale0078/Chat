using System.Windows;

namespace Chat.Client.ViewModel
{
    public class UserInfoVisibilityViewModel : ViewModelBase
    {
        private Visibility _backgroundInfoVisibility;

        public UserInfoVisibilityViewModel()
        {
            BackgroundInfoVisibility = Visibility.Collapsed;
        }

        public Visibility BackgroundInfoVisibility 
        {
            get => _backgroundInfoVisibility;
            set 
            {
                _backgroundInfoVisibility = value;

                OnPropertyChanged();
            }
        }
    }
}
