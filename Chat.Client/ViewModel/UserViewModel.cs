using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Chat.Client.Services;
using Chat.Client.Services.Interfaces;
using Chat.Client.Commands;

namespace Chat.Client.ViewModel
{
    public class UserViewModel : ViewModelBase
    {
        private readonly ChatService _chatService;
        private readonly IDialogService _dialog;

        private string _id;
        private string _name;
        private string _message;
        private byte[] _photo;
        private bool _isAdmin;
        private bool _isMuted;
        private bool _isButtonEnabled;

        private ICommand _setNewPhoto;

        public UserViewModel(ChatService chatService, IDialogService dialog)
        {
            _chatService = chatService;
            _dialog = dialog;
        }

        public string Id 
        {
            get => _id;
            set 
            {
                _id = value;

                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;

                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;

                if (string.IsNullOrEmpty(_message)
                    || string.IsNullOrWhiteSpace(_message)
                    || IsMuted)
                {
                    IsButtonEnabled = false;
                }
                else
                {
                    IsButtonEnabled = true;
                }

                OnPropertyChanged();
            }
        }

        public byte[] Photo 
        {
            get => _photo;
            set 
            {
                _photo = value;

                OnPropertyChanged();
            }
        }

        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;

                OnPropertyChanged();
            }
        }

        public bool IsMuted 
        {
            get => _isMuted;
            set 
            {
                _isMuted = value;
                IsButtonEnabled = value;

                OnPropertyChanged();
            }
        }

        public bool IsButtonEnabled 
        {
            get => _isButtonEnabled;
            set 
            {
                _isButtonEnabled = value;

                OnPropertyChanged();
            }
        }

        public ICommand SetNewPhoto => _setNewPhoto ?? (_setNewPhoto = new RelayCommandAsync(
            execute: ExecuteSetNewPhoto));

        private async Task ExecuteSetNewPhoto(object parametr) 
        {
            string source = _dialog.OpenFile("Choose image", "Images (*.jpg;*png)|*.jpg;*png");

            if (string.IsNullOrEmpty(source))
            {
                MessageBox.Show("Invaled photo");//ToDo: Custom message box

                return;
            }

            Photo = await File.ReadAllBytesAsync(source);

            await _chatService.SendUserPhotoToAllUsersExceptChanged(Name, Photo);
        }
    }
}
