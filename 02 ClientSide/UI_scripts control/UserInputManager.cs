using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UserInputManager : MonoBehaviour
{
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_InputField regEmailField;
    public TMP_InputField regPasswordField;
    public TMP_InputField regPasswordConfirm;
    public Toggle conditionTerminsCheck;
    public Toggle forceLoginRequested;

    public UserInput GetUserInput(string keyType)
    {

        if (keyType == CommandKeys.LoginRequest)
        {
            return new UserInput(emailLoginField.text, passwordLoginField.text);
        }
        else if (keyType == CommandKeys.RegistrationRequest)
        {
            return new UserInput(regEmailField.text, regPasswordField.text, regPasswordConfirm.text);
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
    }
    public UserInput(string email, string password, string confirmPassword)
    {
        Email = email;
        Password = password;
        ConfirmPassword = confirmPassword;
    }
}

