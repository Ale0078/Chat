namespace Chat.Client.ViewModel.Base
{
    public abstract class UserViewModelBase : ViewModelBase
    {
        private string _name;
        private byte[] _photo;
        
        public string Name 
        {
            get => _name;
            set 
            {
                _name = value;

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
    }
}
