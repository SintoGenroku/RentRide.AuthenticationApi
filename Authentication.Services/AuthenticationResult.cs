namespace Authentication.Services;

public class AuthenticationResult
{

        private readonly ICollection<string> _errorMessages;

        public ICollection<string> ErrorMessages
        {
            get
            {
                if(!IsSuccessfull)
                {
                    return _errorMessages;
                }

                throw new InvalidOperationException("Successfull");
            }
        }
        public bool IsSuccessfull { get; }

        public AuthenticationResult(ICollection<string> errorMessages, bool isSuccessfull)
        {
            _errorMessages = errorMessages;
            IsSuccessfull = isSuccessfull;
        }

        public static AuthenticationResult RegistrationSuccessful()
        {
            return new AuthenticationResult(null, true);
        }

        public static AuthenticationResult RegistrationFailed(ICollection<string> errorMessages)
        {
            if (errorMessages == null || !errorMessages.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(errorMessages), "No error messages");
            }

            return new AuthenticationResult(errorMessages, false);
        }
        public static AuthenticationResult SignInFailed(ICollection<string> errorMessages)
        {
            if (errorMessages == null || !errorMessages.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(errorMessages), "No error messages");
            }

            return new AuthenticationResult(errorMessages, false);
        }
}