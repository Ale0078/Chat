﻿using ReactiveValidation;
using ReactiveValidation.Extensions;

namespace Chat.Client.ViewModel
{
    public class RegistarationViewModel : ValidatableObject
    {
        private const int MAX_LENGTH_OF_NAME = 15;
        private const int MIN_LINGTH_OF_PASSWORD = 6;

        private string _name;
        private string _password;
        private string _confirmPassword;

        public RegistarationViewModel()
        {
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
    }
}
