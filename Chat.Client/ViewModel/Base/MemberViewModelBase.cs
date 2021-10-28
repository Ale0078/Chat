using System.Collections.ObjectModel;

namespace Chat.Client.ViewModel.Base
{
    public abstract class MemberViewModelBase : UserViewModelBase
    {
        private DraftViewModel _draft;
        private MessageViewModelBase _lastMessage;
        private MessageViewModelBase _editMessage;

        public MemberViewModelBase()
        {
            Draft = new DraftViewModel();
            Messages = new ObservableCollection<MessageViewModelBase>();
        }

        public ObservableCollection<MessageViewModelBase> Messages { get; set; }
        public bool IsGroup { get; set; }

        public DraftViewModel Draft
        {
            get => _draft;
            set
            {
                _draft = value;

                OnPropertyChanged();
            }
        }

        public MessageViewModelBase LastMessage
        {
            get => _lastMessage;
            set
            {
                _lastMessage = value;

                OnPropertyChanged();
            }
        }

        public MessageViewModelBase EditMessage
        {
            get => _editMessage;
            set
            {
                _editMessage = value;

                OnPropertyChanged();
            }
        }
    }
}
