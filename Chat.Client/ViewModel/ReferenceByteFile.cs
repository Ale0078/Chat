using Chat.Client.ViewModel.Base;

namespace Chat.Client.ViewModel
{
    public class ReferenceByteFile : ViewModelBase
    {
        private byte[] _byteFile;

        public byte[] ByteFile 
        {
            get => _byteFile;
            set 
            {
                _byteFile = value;

                OnPropertyChanged();
            }
        }
    }
}
