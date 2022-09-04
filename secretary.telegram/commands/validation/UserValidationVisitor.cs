using secretary.storage.models;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands.validation;

public class UserValidationVisitor
{
    public void Validate(User? user)
    {
        
        if (user == null)
        {
            throw new NonCompleteUserException("User does not exist");
        }

        if (user.AccessToken == null)
        {
            throw new NonCompleteUserException("User has not got registered email");
        }

        if (user.JobTitleGenitive == null)
        {
            throw new NonCompleteUserException("User has not got personal info");
        }
    } 
}