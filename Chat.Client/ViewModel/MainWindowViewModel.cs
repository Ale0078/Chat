using System.Threading.Tasks;
using System.Windows.Input;

using Chat.Client.Services;
using Chat.Client.Commands;
using Chat.Models;
using Chat.Client.ViewModel.Base;

namespace Chat.Client.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly RegistrationChatService _registrationChatService;
        private readonly ChatService _chatService;

        private UserState _state;

        private ICommand _connectRegistrationChatService;

        public MainWindowViewModel(ChatService chatService, RegistrationChatService registrationChatService, 
            LoginViewModel loginViewModel, RegistarationViewModel registarationViewModel, ChatViewModel chatViewModel)
        {
            _chatService = chatService;
            _registrationChatService = registrationChatService;

            Registaration = registarationViewModel;
            Login = loginViewModel;
            Chat = chatViewModel;

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
            await _registrationChatService.ConnectAsync();
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
            _chatService.SetToken(token);
        }

        private void SendUserStateEventHandler(UserState state) 
        {
            State = state;
        }
    }
}
