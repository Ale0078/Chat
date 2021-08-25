using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ReactiveValidation;
using ReactiveValidation.Extensions;

namespace Chat.Client.ViewModel
{
    public class RegistarationViewModel : ValidatableObject
    {
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

        private IObjectValidator GetValidator() 
        {
            ValidationBuilder<RegistarationViewModel> validatorBuilder = new();

            validatorBuilder.RuleFor(model => model.Name)
                .NotEmpty()
                .WithMessage("Your name cannot be emty");

            return validatorBuilder.Build(this);
        }
    }
}
