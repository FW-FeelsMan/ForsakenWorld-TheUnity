using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Diagnostics;
using UnityEngine;

public class RequestToServer : MonoBehaviour
{
    private UIManager uIManager;
    private UserInputManager userInputManager;
    private static Stopwatch pingTimer = new Stopwatch();


    void Start()
    {
        uIManager = GetComponent<UIManager>();
        userInputManager = GetComponent<UserInputManager>();
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
            ThreadSafeLogger.Log("Unknown request type.");
        }

        if (keyType == CommandKeys.LoginRequest && !EmailValidator.IsValidEmail(userInput.Email))
        {
            isValid = false;
            errorMessage = GlobalStrings.IncorrectEmail;
            ThreadSafeLogger.Log("Invalid email format for login request.");
        }
        else if (keyType == CommandKeys.RegistrationRequest)
        {
            if (!EmailValidator.IsValidEmail(userInput.Email))
            {
                isValid = false;
                errorMessage = GlobalStrings.IncorrectEmail;
                ThreadSafeLogger.Log("Invalid email format for registration request.");
            }
            else if (!EmailValidator.IsValidPassword(userInput.Password))
            {
                isValid = false;
                errorMessage = GlobalStrings.IncorrectPassword;
                ThreadSafeLogger.Log("Invalid password format for registration request.");
            }
            else if (userInput.Password != userInput.ConfirmPassword)
            {
                isValid = false;
                errorMessage = GlobalStrings.PasswordMismatch;
                ThreadSafeLogger.Log("Password mismatch in registration request.");
            }
            else if (!userInputManager.conditionTerminsCheck.isOn)
            {
                isValid = false;
                errorMessage = GlobalStrings.UserAgreementFail;
                ThreadSafeLogger.Log("User agreement not accepted in registration request.");
            }
        }

        if (!isValid)
        {
            uIManager.DisplayError(errorMessage);
        }
        else
        {

            var task = UserData(keyType, userInput.Email, userInput.Password, userInputManager.forceLoginRequested.isOn);
            yield return new WaitUntil(() => task.IsCompleted);

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

    public static async Task SendPingMessage()
    {
        pingTimer.Restart(); 

        var dataObject = new GlobalDataClasses.ClientResponseMessage
        {
            KeyType = CommandKeys.GetPing,
            Message = "Ping"
        };

        await RequestTypeAsync(CommandKeys.GetPing, dataObject);
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

            return requestData;
        }
        catch (Exception ex)
        {
            ThreadSafeLogger.Log($"Error in RequestTypeAsync: {ex.Message}");
            return null;
        }
    }
}