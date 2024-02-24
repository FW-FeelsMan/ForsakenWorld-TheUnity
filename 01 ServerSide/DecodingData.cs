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
    private readonly int clientSocketNum;
    public int currentActiveSocketNum;

    public DecodingData(Socket clientSocket)
    {
        answerToClient = new AnswerToClient(clientSocket);
        IntPtr handle = clientSocket.Handle;
        clientSocketNum = handle.ToInt32();
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
        if (!(dataObject is GlobalDataClasses.UserDataObject userData))
        {
            _ = answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.UnknownPackage);
            return;
        }

        string email = userData.Email;
        string hashedPassword = userData.HashedPassword;
        string hardwareID = userData.HardwareID;
        bool forceLoginRequested = userData.ForceLoginRequested;

        if (email == null || hashedPassword == null || hardwareID == null)
        {
            _ = answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.IncorrectData);
            return;
        }

        bool loginResult = dataHandler.HandleLoginData(email, hashedPassword, hardwareID);

        if (!loginResult)
        {
            _ = answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.FailedLogin);
            return;
        }

        if (!int.TryParse(dataHandler.IsUserActive(email), out int status))
        {
            Debug.Log($"{status} Не удалось преобразовать статус в целое число");
            return;
        }

        if (status == 1)
        {
            if (forceLoginRequested)
            {
                GetClientSocket(email);
                if (clientSocketNum != currentActiveSocketNum)
                {
                    Socket clientSocket = SocketServer.connectedClients.Find(socket => ((int)socket.Handle) == currentActiveSocketNum);

                    if (clientSocket != null)
                    {
                        SocketServer.Instance.RemoveClient(clientSocket);
                        Debug.Log($"Произошел принудительный вход");
                        _ = answerToClient.ServerResponseWrapper(CommandKeys.DisconnectKey, GlobalStrings.ForcedLogin);
                        return;
                    }
                    else
                    {
                        Debug.LogError($"Socket с номером {currentActiveSocketNum} не найден.");
                    }
                }
            }
            else
            {
                _ = answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.UserIsAlreadyOnline);
                return;
            }
        }

        dataHandler.SetClientStatus(email, 1);
        dataHandler.SetSocketClient(email, clientSocketNum);
        _ = answerToClient.ServerResponseWrapper(CommandKeys.SuccessfulLogin, GlobalStrings.WelcomeMessage);
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
    public void GetClientSocket(string email)
    {
        int socketNum = dataHandler.GetClientSocketNum(email);
        currentActiveSocketNum = socketNum;
    }

    public async Task ProcessPacketAsync(byte[] packet)
    {
        try
        {
            using MemoryStream memoryStream = new MemoryStream(packet);
            var formatter = new BinaryFormatter();
            string keyType = (string)formatter.Deserialize(memoryStream);

            if (handlers.TryGetValue(keyType, out var handler))
            {
                object dataObject = formatter.Deserialize(memoryStream);

                // Вместо Task.Run используем асинхронный вызов внутри главного потока Unity
                await Task.Yield(); // Освобождаем поток на одну итерацию цикла обновления

                handler(dataObject);
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Ошибка обработки пакета: {ex.Message}");
        }
    }


}
