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
    public event Action<int> ClientDisconnected;

    private void OnClientDisconnected(int socketNum)
    {
        ClientDisconnected?.Invoke(socketNum);
        ThreadSafeLogger.Log($"Client disconnected event triggered for socket: {socketNum}");
    }

    public DecodingData(Socket clientSocket)
    {
        answerToClient = new AnswerToClient(clientSocket);
        IntPtr handle = clientSocket.Handle;
        clientSocketNum = handle.ToInt32();
        //ThreadSafeLogger.Log($"DecodingData initialized for client socket: {clientSocket.RemoteEndPoint}");
        PacketHandlers();
    }

    private void PacketHandlers()
    {
        PacketHandler(CommandKeys.LoginRequest, HandleLoginRequest);
        PacketHandler(CommandKeys.RegistrationRequest, HandleRegistrationRequest);

        //ThreadSafeLogger.Log("Packet handlers registered.");
    }

    private void PacketHandler(string keyType, Action<object> handler)
    {
        handlers[keyType] = handler;
        //ThreadSafeLogger.Log($"Packet handler registered for key: {keyType}");
    }

    private async void HandleLoginRequest(object dataObject)
    {
        if (!(dataObject is GlobalDataClasses.UserDataObject userData))
        {
            ThreadSafeLogger.Log("Invalid login request data.");
            await answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.UnknownPackage);
            return;
        }

        string email = userData.Email;
        string hashedPassword = userData.HashedPassword;
        string hardwareID = userData.HardwareID;
        bool forceLoginRequested = userData.ForceLoginRequested;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(hardwareID))
        {
            ThreadSafeLogger.Log("Login request with missing data.");
            await answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.IncorrectData);
            return;
        }

        bool loginResult = await dataHandler.HandleLoginDataAsync(email, hashedPassword, hardwareID, forceLoginRequested);

        if (!loginResult)
        {
            ThreadSafeLogger.Log($"Failed login attempt for email: {email}");
            await answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.FailedLogin);
            return;
        }

        int userId = await dataHandler.GetUserIdByEmailAsync(email);
        if (!int.TryParse(await dataHandler.IsUserActiveAsync(email), out int status))
        {
            ThreadSafeLogger.Log($"Failed to convert user status to integer for email: {email}");
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
                        ThreadSafeLogger.Log($"Forced login for email: {email}");
                        await answerToClient.ServerResponseWrapper(CommandKeys.DisconnectKey, GlobalStrings.ForcedLogin);
                    }
                    else
                    {
                        ThreadSafeLogger.Log($"Socket with number {currentActiveSocketNum} not found.");
                    }
                }
            }
            else
            {
                ThreadSafeLogger.Log($"User already online: {email}");
                await answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.UserIsAlreadyOnline);
                return;
            }
        }

        await dataHandler.SetClientStatusAsync(email, 1);
        await dataHandler.SetSocketClientAsync(email, clientSocketNum);
        await answerToClient.ServerResponseWrapper(CommandKeys.SuccessfulLogin, GlobalStrings.WelcomeMessage);
        ThreadSafeLogger.Log($"User logged in successfully: {email}");
    }

    private async void HandleRegistrationRequest(object dataObject)
    {
        if (dataObject is GlobalDataClasses.UserDataObject userData)
        {
            string email = userData.Email;
            string hashedPassword = userData.HashedPassword;
            string hardwareID = userData.HardwareID;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(hardwareID))
            {
                ThreadSafeLogger.Log("Registration request with missing data.");
                await answerToClient.ServerResponseWrapper(CommandKeys.FailedRegistration, GlobalStrings.IncorrectData);
            }
            else
            {
                bool registerResult = await dataHandler.HandleRegistrationDataAsync(email, hashedPassword, hardwareID);

                if (registerResult)
                {
                    await answerToClient.ServerResponseWrapper(CommandKeys.SuccessfulRegistration, GlobalStrings.SuccessfulRegistration);
                    ThreadSafeLogger.Log($"User registered successfully: {email}");
                }
                else
                {
                    ThreadSafeLogger.Log($"Registration error - email exists: {email}");
                    await answerToClient.ServerResponseWrapper(CommandKeys.FailedRegistration, GlobalStrings.RegistrationErrorEmailExists);
                }
            }
        }
    }

    public async void ClientisDisconnected()
    {
        try
        {
            var emailUserInLogged = dataHandler.GetLoggedInUserEmail();
            if (!string.IsNullOrEmpty(emailUserInLogged))
            {
                await dataHandler.SetClientStatusAsync(emailUserInLogged, 0);
                ThreadSafeLogger.Log($"Client status set to offline for email: {emailUserInLogged}");
            }
        }
        catch (Exception ex)
        {
            ThreadSafeLogger.Log($"Ошибка при отключении клиента: {ex.Message}");
        }
    }

    public void GetClientSocket(string email)
    {
        currentActiveSocketNum = dataHandler.GetClientSocketNumAsync(email).Result;
        ThreadSafeLogger.Log($"Retrieved client socket number for email: {email} - {currentActiveSocketNum}");
    }


    public async Task ProcessPacketAsync(byte[] packet)
    {
        await Task.Run(() =>
        {
            using MemoryStream memoryStream = new(packet);
            var formatter = new BinaryFormatter();

            string keyType = (string)formatter.Deserialize(memoryStream);

            if (handlers.TryGetValue(keyType, out var handler))
            {
                object dataObject = formatter.Deserialize(memoryStream);
                //ThreadSafeLogger.Log($"Processing packet with key: {keyType}");
                handler(dataObject);
            }
            else
            {
                ThreadSafeLogger.Log($"No handler found for key: {keyType}");
            }
        });
    }
}