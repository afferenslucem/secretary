using Secretary.Storage.Models;
using Secretary.Telegram.Exceptions;

namespace Secretary.Telegram.Commands.Validation;

public class UserValidationVisitor
{
    public void Validate(User? user)
    {
        
        if (user == null || (user.Email == null && user.Name == null))
        {
            throw new NonCompleteUserException("User does not exist");
        }

        if (user.Email == null)
        {
            throw new NonCompleteUserException("User has not got registered email");
        }

        if (user.Name == null)
        {
            throw new NonCompleteUserException("User has not got personal info");
        }
    } 
}