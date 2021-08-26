using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveValidation;
using ReactiveValidation.Extensions;

using Chat.Client.Services;
using Chat.Client.Commands;
using Chat.Models;

namespace Chat.Client.ViewModel
{
    public class RegistarationViewModel : ValidatableObject
    {
        private const int MAX_LENGTH_OF_NAME = 15;
        private const int MIN_LINGTH_OF_PASSWORD = 6;

        private readonly RegistrationChatService _registrationService;

        private string _name;
        private string _password;
        private string _confirmPassword;

        private ICommand _registerUser;

        public RegistarationViewModel()
        {
            _registrationService = new RegistrationChatService();

            Validator = GetValidator();
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

        public string Password
        {
            get => _password;
            set
            {
                _password = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;

                OnPropertyChanged();
            }
        }

        public ICommand RegisterUser => _registerUser ?? (_registerUser = new RelayCommandAsync(
            execute: ExecuteRegistrationUser));

        private bool ConfirmPasswords(string confirmPassword) =>
            confirmPassword == Password;

        private IObjectValidator GetValidator() 
        {
            ValidationBuilder<RegistarationViewModel> validatorBuilder = new();

            validatorBuilder.RuleFor(model => model.Name)
                .NotEmpty()
                .WithMessage("Your name cannot be emty")
                .Matches(@"^[A-Za-z0-9]+$")
                .WithMessage("Name can contain only alphabetic chars and numbers and cannot be longer than 15 chars")
                .MaxLength(MAX_LENGTH_OF_NAME)
                .WithMessage("Name cannot be longer than 15 chars");

            validatorBuilder.RuleFor(model => model.Password)
                .MinLength(MIN_LINGTH_OF_PASSWORD)
                .WithMessage("Pssword cannot be less than 6 chars");

            validatorBuilder.RuleFor(model => model.ConfirmPassword)
                .Must(ConfirmPasswords)
                .WithMessage("Confirm password has to be the same as 'Password'");

            return validatorBuilder.Build(this);
        }

        private async Task ExecuteRegistrationUser(object parameters) 
        {
            await _registrationService.Connect();

            await _registrationService.RegisterUser(new RegisterUserModel
            {
                UserName = Name,
                Password = Password,
                PasswordConfirm = ConfirmPassword
            });
        }
    }
}
