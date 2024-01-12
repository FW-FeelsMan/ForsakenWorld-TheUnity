using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ForsakenWorld;

public class DecodingData
{
    private readonly Dictionary<string, Action<object>> handlers = new();
    private readonly DataHandler dataHandler;
    private readonly AnswerToClient answerToClient;

    public DecodingData()
    {
        RegisterHandlers();
        dataHandler = new DataHandler();
        answerToClient = new AnswerToClient();
    }

    private void RegisterHandlers()
    {
        RegisterHandler(CommandKeys.LoginRequest, HandleLoginRequest);
        RegisterHandler(CommandKeys.RegistrationRequest, HandleRegistrationRequest);
    }

    private void RegisterHandler(string keyType, Action<object> handler)
    {
        handlers[keyType] = handler;
    }

    private void HandleLoginRequest(object dataObject)
    {
        if (dataObject is GlobalDataClasses.UserDataObject userData)
        {
            string email = userData.Email;
            string hashedPassword = userData.HashedPassword;
            string hardwareID = userData.HardwareID;
            DateTime lastLoginDate = DateTime.Now;

            if (email == null || hashedPassword == null || hardwareID == null)
            {
                string exceptionMessage = "������������ ������/������ �������� ������������ �������";
                answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, exceptionMessage);
            }
            else
            {
                bool loginResult = dataHandler.HandleLoginData(email, hashedPassword, hardwareID, lastLoginDate);

                if (loginResult)
                {
                    dataHandler.IsUserActive(email);

                    if (dataHandler._isUserActive)
                    {
                        string exceptionMessage = "������������ ��� ������";
                        answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, exceptionMessage);
                    }
                    else
                    {
                        dataHandler.SetClientStatus(email, 1);
                        string responseMessage = "����� ����������!";
                        answerToClient.ServerResponseWrapper(CommandKeys.LoginRequest, responseMessage);
                    }
                }
                else
                {
                    string exceptionMessage = "��������� ������� �����. ��������� ����� � ������";
                    answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, exceptionMessage);
                }
            }
        }
    }

    private void HandleRegistrationRequest(object dataObject)
    {
        if (dataObject is GlobalDataClasses.UserDataObject userData)
        {
            string email = userData.Email;
            string hashedPassword = userData.HashedPassword;
            string hardwareID = userData.HardwareID;
            DateTime lastLoginDate = DateTime.Now;

            if (email == null || hashedPassword == null || hardwareID == null)
            {
                string exceptionMessage = "������������ ������";
                answerToClient.ServerResponseWrapper(CommandKeys.FailedRegistration, exceptionMessage);
            }
            else
            {
                bool registerResult = dataHandler.HandleRegistrationData(email, hashedPassword, hardwareID, lastLoginDate);

                if (registerResult)
                {
                    string responseMessage = "����������� �������";
                    answerToClient.ServerResponseWrapper(CommandKeys.SuccessfulRegistration, responseMessage);
                }
                else
                {
                    string exceptionMessage = "��������� ����� ��� ���������������";
                    answerToClient.ServerResponseWrapper(CommandKeys.FailedRegistration, exceptionMessage);
                }
            }
        }
    }

    public void ClientisDisconnected()
    {
        var emailUserInLogged = dataHandler.GetLoggedInUserEmail();
        dataHandler.SetClientStatus(emailUserInLogged, 0);
    }

    public void ProcessPacketAsync(byte[] packet)
    {
        try
        {
            using MemoryStream memoryStream = new(packet);
            var formatter = new BinaryFormatter();

            try
            {
                string keyType = (string)formatter.Deserialize(memoryStream);

                if (handlers.TryGetValue(keyType, out var handler))
                {
                    object dataObject = formatter.Deserialize(memoryStream);
                    handler(dataObject);
                }
                else
                {
                    LogProcessor.ProcessLog(FWL.GetClassName(), $"�� ������ ���������� ��� ���������� ����: {keyType}");
                }
            }
            catch (Exception ex)
            {
                LogProcessor.ProcessLog(FWL.GetClassName(), $"������ �������������� �������: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"������ ��������� ����������� ������: {ex.Message}");
        }
    }
}
