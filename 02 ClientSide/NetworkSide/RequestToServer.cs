using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
public class RequestToServer : MonoBehaviour
{
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_InputField reg_emailLoginField;
    public TMP_InputField reg_passwordLoginField;
    public TMP_InputField reg_passwordConfirm;
    private UIManager uIManager;
    private static string thisClassName;
    void Start()
    {
        uIManager = GetComponent<UIManager>();
        thisClassName = GetType().Name;
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

        string email = "";
        string password = "";

        if (keyType == CommandKeys.LoginRequest)
        {
            if (!EmailValidator.IsValidEmail(emailLoginField.text))
            {
                isValid = false;
                errorMessage = "Некорректный емейл!";
            }
            else if (string.IsNullOrWhiteSpace(emailLoginField.text) || string.IsNullOrWhiteSpace(passwordLoginField.text))
            {
                isValid = false;
                errorMessage = "Поля емейла и пароля не могут быть пустыми!";
            }
            
            email = emailLoginField.text;
            password = passwordLoginField.text;
        }
        else if (keyType == CommandKeys.RegistrationRequest)
        {
            if (!EmailValidator.IsValidEmail(reg_emailLoginField.text))
            {
                isValid = false;
                errorMessage = "Некорректный емейл!";
            }
            else
            {
                if (!EmailValidator.IsValidPassword(reg_passwordLoginField.text))
                {
                    isValid = false;
                    errorMessage = "Пароль менее 6 символов или содержит недопустимые символы!";
                }
                else if (reg_passwordLoginField.text != reg_passwordConfirm.text)
                {
                    isValid = false;
                    errorMessage = "Пароли не совпадают!";
                }
            }

            email = reg_emailLoginField.text;
            password = reg_passwordLoginField.text;
        }
        else
        {
            isValid = false;
            errorMessage = "Неизвестный тип запроса!";
        }

        if (!isValid)
        {
            uIManager.DisplayError(errorMessage);
        }
        else
        {
            var task = UserData(keyType, email, password);
            yield return new WaitUntil(() => task.IsCompleted);
        }
    }

    public static async Task<byte[]> UserData(string _keyType, string _email, string _password)
    {
        var _hashedPassword = HashData.Hashing(_password);
        var _hardwareID = HardwareID.GetHardwareID();

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
            
            LogProcessor.ProcessLog(thisClassName, $"Error in RequestTypeAsync: {ex.Message}");
            return null;
        }
    }
}
