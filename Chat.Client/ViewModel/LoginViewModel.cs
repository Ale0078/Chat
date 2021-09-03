using System;
using System.Threading.Tasks;
using System.Windows.Input;

using ReactiveValidation;
using ReactiveValidation.Extensions;

using Chat.Client.Services;
using Chat.Client.Commands;
using Chat.Models;

namespace Chat.Client.ViewModel
{
    public class LoginViewModel : ValidatableObject
    {
        private const int MAX_LENGTH_OF_NAME = 15;
        private const int MIN_LINGTH_OF_PASSWORD = 6;

        private readonly RegistrationChatService _registrationChatService;

        private string _userName;
        private string _password;

        private ICommand _loginUser;
        private ICommand _setRegistrationTemplate;

        public event Action<UserState> SetStateEvent;

        public LoginViewModel(RegistrationChatService registrationChatService)
        {
            _registrationChatService = registrationChatService;

            Validator = GetValidator();
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

        public string Password 
        {
            get => _password;
            set 
            {
                _password = value;

                OnPropertyChanged();
            }
        }

        public ICommand LoginUser => _loginUser ?? (_loginUser = new RelayCommandAsync(
            execute: LoginUserExecute));

        private async Task LoginUserExecute(object parametr)
        {
            LoginResult result = await _registrationChatService.LoginUser(UserName, Password);
        }

        public ICommand SetRegistrationTemplate => _setRegistrationTemplate ?? (_setRegistrationTemplate = new RelayCommand(
            execute: ExecuteSetRegistrationTemplate));

        private void ExecuteSetRegistrationTemplate(object parametr) 
        {
            SetStateEvent?.Invoke(UserState.NoRegistered);
        }

        private IObjectValidator GetValidator() 
        {
            ValidationBuilder<LoginViewModel> validator = new();

            validator.RuleFor(model => model.UserName)
                .NotEmpty()
                .WithMessage("Your name cannot be emty")
                .Matches(@"^[A-Za-z0-9]+$")
                .WithMessage("Name can contain only alphabetic chars and numbers and cannot be longer than 15 chars")
                .MaxLength(MAX_LENGTH_OF_NAME)
                .WithMessage("Name cannot be longer than 15 chars");

            validator.RuleFor(model => model.Password)
                .MinLength(MIN_LINGTH_OF_PASSWORD)
                .WithMessage("Pssword cannot be less than 6 chars");

            return validator.Build(this);
        }
    }
}
