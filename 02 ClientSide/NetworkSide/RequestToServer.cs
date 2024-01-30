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
        }

        if (keyType == CommandKeys.LoginRequest && !EmailValidator.IsValidEmail(userInput.Email))
        {
            isValid = false;
            errorMessage = GlobalStrings.IncorrectEmail;
        }
        else if (keyType == CommandKeys.RegistrationRequest)
        {
            if (!EmailValidator.IsValidEmail(userInput.Email))
            {
                isValid = false;
                errorMessage = GlobalStrings.IncorrectEmail;
            }
            else if (!EmailValidator.IsValidPassword(userInput.Password))
            {
                isValid = false;
                errorMessage = GlobalStrings.IncorrectPassword;
            }
            else if (userInput.Password != userInput.ConfirmPassword)
            {
                isValid = false;
                errorMessage = GlobalStrings.PasswordMismatch;
            }
            else if (!userInputManager.ConditionTerminsCheck.isOn)
            {
                isValid = false;
                errorMessage = GlobalStrings.UserAgreementFail;
            }
        }

        if (!isValid)
        {
            uIManager.DisplayError(errorMessage);
        }
        else
        {
            var task = UserData(keyType, userInput.Email, userInput.Password);
            yield return new WaitUntil(() => task.IsCompleted);
        }
    }

    public static async Task<byte[]> UserData(string _keyType, string _email, string _password)
    {
        var _hashedPassword = GlobalStrings.Hashing(_password);
        var _hardwareID = GlobalStrings.GetHardwareID();

        var _dataObject = new GlobalDataClasses.UserDataObject
        {
            KeyType = _keyType,
            Email = _email,
            HashedPassword = _hashedPassword,
            HardwareID = _hardwareID
        };

        return await RequestTypeAsync(_keyType, _dataObject);
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

            SocketClient.Instance.SendData(requestData);

            return requestData;
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"Error in RequestTypeAsync: {ex.Message}");
            return null;
        }
    }
}
