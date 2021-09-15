using System.Threading.Tasks;
using System.Windows.Input;

using Chat.Client.Services;
using Chat.Client.Commands;
using Chat.Models;

namespace Chat.Client.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly RegistrationChatService _registrationChatService;
        private readonly ChatService _chatService;

        private UserState _state;

        private ICommand _connectRegistrationChatService;

        public MainWindowViewModel()
        {
            _chatService = new ChatService();
            _registrationChatService = new RegistrationChatService();

            Registaration = new RegistarationViewModel(_registrationChatService);
            Login = new LoginViewModel(_registrationChatService);
            Chat = new ChatViewModel(_chatService, _registrationChatService);

            State = UserState.NoLogin;

            SetEvents();
        }

        public RegistarationViewModel Registaration { get; }
        public LoginViewModel Login { get; }
        public ChatViewModel Chat { get; }

        public UserState State 
        {
            get => _state;
            set
            {
                _state = value;

                OnPropertyChanged();
            }
        }

        public ICommand ConnectRegistrationChatService => _connectRegistrationChatService ?? (_connectRegistrationChatService = new RelayCommandAsync(
            execute: ConnectRegistrationChatServiceEventHandler));

        private async Task ConnectRegistrationChatServiceEventHandler(object parametr)
        {
            await _registrationChatService.Connect();
        }

        private void SetEvents() 
        {
            _registrationChatService.SendTokenToCallerServerHandler += SendTokenEventHandler;
            _registrationChatService.SendUserStateToCallerServerHandler += SendUserStateEventHandler;

            _chatService.SetBlockedStateUserToBlockedUser += SendUserStateEventHandler;

            Login.SetStateEvent += SendUserStateEventHandler;
        }

        private void SendTokenEventHandler(string token) 
        {
            _chatService.Token = token;
        }

        private void SendUserStateEventHandler(UserState state) 
        {
            State = state;
        }
    }
}
