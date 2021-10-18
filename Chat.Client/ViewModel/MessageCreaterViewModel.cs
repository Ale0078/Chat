﻿using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

using Chat.Client.Services.Interfaces;
using Chat.Client.Commands;

namespace Chat.Client.ViewModel
{
    public class MessageCreaterViewModel : ViewModelBase
    {
        private readonly IDialogService _dialog;

        private string _textMessage;
        private byte[] _fileMessage;

        private ICommand _setFileMessage;
        private ICommand _cancel;
        
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

            FileMessage = await File.ReadAllBytesAsync(source);
        }

        public ICommand Canecl => _cancel ??= new RelayCommand(
            execute: ExecuteCancel);

        private void ExecuteCancel(object parametr) 
        {
            TextMessage = string.Empty;
            FileMessage = null;
        }
    }
}
