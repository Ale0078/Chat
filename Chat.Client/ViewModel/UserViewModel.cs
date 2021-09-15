using System.Windows;

namespace Chat.Client.ViewModel
{
    public class UserViewModel : ViewModelBase
    {
        private string _id;
        private string _userName;
        private string _message;
        private bool _isAdmin;
        private Visibility _adminToolsVisibility;
        private Visibility _buttonToSendVisibility;

        public string Id 
        {
            get => _id;
            set 
            {
                _id = value;

                OnPropertyChanged();
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;

                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;

                if (_message == string.Empty
                    || string.IsNullOrEmpty(_message)
                    || string.IsNullOrWhiteSpace(_message))
                {
                    ButtonToSendVisibility = Visibility.Collapsed;
                }
                else
                {
                    ButtonToSendVisibility = Visibility.Visible;
                }

                OnPropertyChanged();
            }
        }

        public Visibility ButtonToSendVisibility
        {
            get => _buttonToSendVisibility;
            set
            {
                _buttonToSendVisibility = value;

                OnPropertyChanged();
            }
        }

        public bool IsAdmin 
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;

                if (IsAdmin)
                {
                    AdminToolsVisibility = Visibility.Visible;
                }
                else
                {
                    AdminToolsVisibility = Visibility.Collapsed;
                }

                OnPropertyChanged();
            }
        }

        public Visibility AdminToolsVisibility
        {
            get => _adminToolsVisibility;
            set
            {
                _adminToolsVisibility = value;

                OnPropertyChanged();
            }
        }
    }
}
