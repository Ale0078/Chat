using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

using Chat.Client.Services.Interfaces;
using Chat.Client.Commands;
using Chat.Client.ViewModel.Base;

namespace Chat.Client.ViewModel
{
    public class MessageCreaterViewModel : ViewModelBase
    {
        private readonly IDialogService _dialog;

        private string _textMessage;
        private byte[] _fileMessage;
        private bool _isPlaceholedApplied;
        private bool _doesHideCreater;
        private bool _doesShowCreater;

        private ICommand _setFileMessage;
        private ICommand _cancel;
        private ICommand _pickEmoji;
        
        public MessageCreaterViewModel(IDialogService dialog)
        {
            _dialog = dialog;
        }

        public string TextMessage 
        {
            get => _textMessage;
            set 
            {
                _textMessage = value;

                OnPropertyChanged();
            }
        }

        public byte[] FileMessage 
        {
            get => _fileMessage;
            set 
            {
                _fileMessage = value;

                OnPropertyChanged();
            }
        }

        public bool IsPlaceholedApplied 
        {
            get => _isPlaceholedApplied;
            set 
            {
                _isPlaceholedApplied = value;

                OnPropertyChanged();
            }
        }

        public bool DoesHideCreater 
        {
            get => _doesHideCreater;
            set 
            {
                _doesHideCreater = value;

                OnPropertyChanged();
            }
        }

        public bool DoesShowCreater 
        {
            get => _doesShowCreater;
            set 
            {
                _doesShowCreater = value;

                OnPropertyChanged();
            }
        }

        public ICommand SetFileMessage => _setFileMessage ??= new RelayCommandAsync(
            execute: ExecuteSetFileMessaage);

        private async Task ExecuteSetFileMessaage(object parametr) 
        {
            TextMessage = string.Empty;

            string source = _dialog.OpenFile("Choose image", "Images (*.jpg;*png)|*.jpg;*png");

            if (string.IsNullOrEmpty(source))
            {
                return;
            }

            DoesShowCreater = true;
            DoesShowCreater = false;

            FileMessage = await File.ReadAllBytesAsync(source);
        }

        public ICommand Canecl => _cancel ??= new RelayCommand(
            execute: ExecuteCancel);

        private void ExecuteCancel(object parametr) 
        {
            TextMessage = string.Empty;
            FileMessage = null;
            DoesHideCreater = true;
            DoesHideCreater = false;
        }

        public ICommand PickEmoji => _pickEmoji ??= new RelayCommand(
            execute: ExecutePickEmoji);

        private void ExecutePickEmoji(object emoji) 
        {
            if (IsPlaceholedApplied)
            {
                IsPlaceholedApplied = false;

                TextMessage = (string)emoji;
            }
            else 
            {
                TextMessage += (string)emoji;
            }
        }
    }
}
