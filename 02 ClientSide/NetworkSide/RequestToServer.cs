using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using ForsakenWorld;
using UnityEngine;

public class RequestToServer : MonoBehaviour
{
    private UIManager uIManager;
    private UserInputManager userInputManager;

    void Start()
    {
        uIManager = GetComponent<UIManager>();
        userInputManager = GetComponent<UserInputManager>();
        Logger.CurrentLogLevel = LogLevel.Debug; // Set the desired log level here
    }

    public void OnLoginButtonClicked()
    {
        StartCoroutine(UserDataWrapper(CommandKeys.LoginRequest));
    }

    public void OnRegistrationButtonClicked()
    {
        StartCoroutine(UserDataWrapper(CommandKeys.RegistrationRequest));
    }

    private IEnumerator UserDataWrapper(string keyType)
    {
        bool isValid = true;
        string errorMessage = "";

        UserInput userInput = userInputManager.GetUserInput(keyType);

        if (userInput == null)
        {
            isValid = false;
            errorMessage = GlobalStrings.UnknownRequestType;
            Logger.Log("Unknown request type.", LogLevel.Warning);
        }

        if (keyType == CommandKeys.LoginRequest && !EmailValidator.IsValidEmail(userInput.Email))
        {
            isValid = false;
            errorMessage = GlobalStrings.IncorrectEmail;
            Logger.Log("Invalid email format for login request.", LogLevel.Warning);
        }
        else if (keyType == CommandKeys.RegistrationRequest)
        {
            if (!EmailValidator.IsValidEmail(userInput.Email))
            {
                isValid = false;
                errorMessage = GlobalStrings.IncorrectEmail;
                Logger.Log("Invalid email format for registration request.", LogLevel.Warning);
            }
            else if (!EmailValidator.IsValidPassword(userInput.Password))
            {
                isValid = false;
                errorMessage = GlobalStrings.IncorrectPassword;
                Logger.Log("Invalid password format for registration request.", LogLevel.Warning);
            }
            else if (userInput.Password != userInput.ConfirmPassword)
            {
                isValid = false;
                errorMessage = GlobalStrings.PasswordMismatch;
                Logger.Log("Password mismatch in registration request.", LogLevel.Warning);
            }
            else if (!userInputManager.ConditionTerminsCheck.isOn)
            {
                isValid = false;
                errorMessage = GlobalStrings.UserAgreementFail;
                Logger.Log("User agreement not accepted in registration request.", LogLevel.Warning);
            }
        }

        if (!isValid)
        {
            uIManager.DisplayError(errorMessage);
        }
        else
        {
            Logger.Log("Sending user data request...", LogLevel.Info);
            var task = UserData(keyType, userInput.Email, userInput.Password, userInputManager.forceLoginRequested.isOn);
            yield return new WaitUntil(() => task.IsCompleted);
            Logger.Log("User data request sent successfully.", LogLevel.Info);
        }
    }

    public static async Task<byte[]> UserData(string keyType, string email, string password, bool forceLoginRequested)
    {
        var hashedPassword = GlobalStrings.Hashing(password);
        var hardwareID = GlobalStrings.GetHardwareID();
        var dataObject = new GlobalDataClasses.UserDataObject
        {
            KeyType = keyType,
            Email = email,
            HashedPassword = hashedPassword,
            HardwareID = hardwareID,
            ForceLoginRequested = forceLoginRequested
        };

        return await RequestTypeAsync(keyType, dataObject);
    }

    public static async Task<byte[]> RequestTypeAsync(string keyType, object dataObject)
    {
        try
        {
            byte[] requestData;
            var formatter = new BinaryFormatter();

            using (MemoryStream memoryStream = new())
            {
                formatter.Serialize(memoryStream, keyType);
                formatter.Serialize(memoryStream, dataObject);

                memoryStream.Position = 0;
                string debugKeyType = (string)formatter.Deserialize(memoryStream);
                object debugDataObject = formatter.Deserialize(memoryStream);

                await memoryStream.FlushAsync();
                requestData = memoryStream.ToArray();
            }

            await SocketClient.Instance.SendData(requestData);
            Logger.Log($"Sent request to server: {keyType}", LogLevel.Info);

            return requestData;
        }
        catch (Exception ex)
        {
            Logger.Log($"Error in RequestTypeAsync: {ex.Message}", LogLevel.Error);
            return null;
        }
    }
}
