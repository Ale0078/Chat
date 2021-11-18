using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Chat.Client.ViewModel.Base
{
    public abstract class MemberViewModelBase : UserViewModelBase
    {
        private DraftViewModel _draft;
        private MessageViewModelBase _lastMessage;
        private MessageViewModelBase _editMessage;
        private ObservableCollection<MessageViewModelBase> _messages;
        private int _countUnreadMessages;

        public MemberViewModelBase()
        {
            Draft = new DraftViewModel();
            Messages = new ObservableCollection<MessageViewModelBase>();

            LastVerticalOffsetToMessages = double.NaN;
        }

        public bool IsGroup { get; set; }

        public bool WasSelected { get; set; }

        public double LastVerticalOffsetToMessages { get; set; }

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

        public ObservableCollection<MessageViewModelBase> Messages 
        {
            get => _messages;
            set 
            {
                if (_messages is not null)
                {
                    _messages.CollectionChanged -= OnMessagesChanged;

                    RemoveCheckerMessagesToBeReadHandlers(_messages);
                }

                CountUnreadMessages = 0;

                _messages = value;

                if (_messages is not null)
                {
                    _messages.CollectionChanged += OnMessagesChanged;

                    CheckMessagesToBeRead(value);
                }
            }
        }

        public int CountUnreadMessages 
        {
            get => _countUnreadMessages;
            set 
            {
                _countUnreadMessages = value;

                OnPropertyChanged();
            }
        }

        private void OnMessagesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ObservableCollection<MessageViewModelBase> messages = sender as ObservableCollection<MessageViewModelBase>;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    for (int i = 1; i <= e.NewItems.Count; i++)
                    {
                        MessageViewModelBase message = messages[^i];

                        if (!message.IsRead && !message.IsFromCurrentUser)
                        {
                            CountUnreadMessages++;
                        }

                        message.PropertyChanged += OnMessageWasRead;
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:

                    throw new NotImplementedException();
            }
        }

        private void OnMessageWasRead(object sender, PropertyChangedEventArgs e) 
        {
            if (e.PropertyName != nameof(MessageViewModelBase.IsRead))
            {
                return;
            }

            MessageViewModelBase message = sender as MessageViewModelBase;

            if (message.IsRead && !message.IsFromCurrentUser)
            {
                CountUnreadMessages--;
            }
        }

        private void CheckMessagesToBeRead(ObservableCollection<MessageViewModelBase> messages) 
        {
            foreach (MessageViewModelBase message in messages)
            {
                if (!message.IsRead && !message.IsFromCurrentUser)
                {
                    CountUnreadMessages++;
                }

                message.PropertyChanged += OnMessageWasRead;
            }
        }

        private void RemoveCheckerMessagesToBeReadHandlers(ObservableCollection<MessageViewModelBase> messages) 
        {
            foreach (MessageViewModelBase message in messages)
            {
                message.PropertyChanged -= OnMessageWasRead;
            }
        }
    }
}
