using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

public class DecodingData : MonoBehaviour
{
    private readonly Dictionary<string, Action<object>> handlers = new();
    private readonly AnswerToClient answerToClient;
    private readonly DataHandler dataHandler = new();

    public DecodingData(Socket clientSocket)
    {
        answerToClient = new AnswerToClient(clientSocket);
        PacketHandlers();
    }

    private void PacketHandlers()
    {
        PacketHandler(CommandKeys.LoginRequest, HandleLoginRequest);
        PacketHandler(CommandKeys.RegistrationRequest, HandleRegistrationRequest);
    }

    private void PacketHandler(string keyType, Action<object> handler)
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
            bool forceLoginRequested = userData.ForceLoginRequested;

            if (email == null || hashedPassword == null || hardwareID == null)
            {
                _ = answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.IncorrectData);
            }
            else
            {
                bool loginResult = dataHandler.HandleLoginData(email, hashedPassword, hardwareID, forceLoginRequested);

                if (loginResult)
                {
                    dataHandler.IsUserActive(email);

                    if (dataHandler._isUserActive)
                    {
                        _ = answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.UserIsAlreadyOnline);
                    }
                    else
                    {
                        if (forceLoginRequested)
                        {
                            // Вызвать метод поиска компьютеров на которых данная учетная запись онлайн
                            // Возможно это можно реализовать через SocketServer
                            // Отсоеденить их от сервера
                        }
                        dataHandler.SetClientStatus(email, 1);
                        _ = answerToClient.ServerResponseWrapper(CommandKeys.SuccessfulLogin, GlobalStrings.WelcomeMessage);
                        Debug.Log(forceLoginRequested);
                    }
                }
                else
                {
                    _ = answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.FailedLogin);
                }
            }
        }
        else
        {
            _ = answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.UnknownPackage);
        }
    }


    private void HandleRegistrationRequest(object dataObject)
    {
        if (dataObject is GlobalDataClasses.UserDataObject userData)
        {
            string email = userData.Email;
            string hashedPassword = userData.HashedPassword;
            string hardwareID = userData.HardwareID;

            if (email == null || hashedPassword == null || hardwareID == null)
            {
                _ = answerToClient.ServerResponseWrapper(CommandKeys.FailedRegistration, GlobalStrings.IncorrectData);
            }
            else
            {
                bool registerResult = dataHandler.HandleRegistrationData(email, hashedPassword, hardwareID);

                if (registerResult)
                {
                    _ = answerToClient.ServerResponseWrapper(CommandKeys.SuccessfulRegistration, GlobalStrings.SuccessfulRegistration);
                }
                else
                {
                    _ = answerToClient.ServerResponseWrapper(CommandKeys.FailedRegistration, GlobalStrings.RegistrationErrorEmailExists);
                }
            }
        }
    }

    public void ClientisDisconnected()
    {
        var emailUserInLogged = dataHandler.GetLoggedInUserEmail();
        dataHandler.SetClientStatus(emailUserInLogged, 0);
    }

    public async Task ProcessPacketAsync(byte[] packet)
    {
        try
        {
            await Task.Run(() =>
            {
                using MemoryStream memoryStream = new(packet);
                var formatter = new BinaryFormatter();
                string keyType = (string)formatter.Deserialize(memoryStream);

                if (handlers.TryGetValue(keyType, out var handler))
                {
                    object dataObject = formatter.Deserialize(memoryStream);
                    handler(dataObject);
                }
            });
        }
        catch (Exception ex)
        {
            Debug.Log($"Ошибка обработки пакета: {ex.Message}");
        }
    }

}
