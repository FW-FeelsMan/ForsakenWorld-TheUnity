using UnityEngine;
using TMPro;

public class UserInputManager : MonoBehaviour
{
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_InputField reg_emailLoginField;
    public TMP_InputField reg_passwordLoginField;
    public TMP_InputField reg_passwordConfirm;

    public UserInput GetUserInput(string keyType)
    {
        if (keyType == CommandKeys.LoginRequest)
        {
            return new UserInput(emailLoginField.text, passwordLoginField.text);
        }
        else if (keyType == CommandKeys.RegistrationRequest)
        {
            return new UserInput(reg_emailLoginField.text, reg_passwordLoginField.text, reg_passwordConfirm.text);
        }
        else
        {
            return null;
        }
    }
}

public class UserInput
{
    public string Email { get; }
    public string Password { get; }
    public string ConfirmPassword { get; }

    public UserInput(string email, string password)
    {
        Email = email;
        Password = password;
        ConfirmPassword = null;
    }

    public UserInput(string email, string password, string confirmPassword)
    {
        Email = email;
        Password = password;
        ConfirmPassword = confirmPassword;
    }
}
