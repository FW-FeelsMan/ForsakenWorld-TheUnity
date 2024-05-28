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
        Logger.Log($"Client disconnected event triggered for socket: {socketNum}", LogLevel.Info);
    }

    public DecodingData(Socket clientSocket)
    {
        answerToClient = new AnswerToClient(clientSocket);
        IntPtr handle = clientSocket.Handle;
        clientSocketNum = handle.ToInt32();
        Logger.Log($"DecodingData initialized for client socket: {clientSocket.RemoteEndPoint}", LogLevel.Info);
        PacketHandlers();
    }

    private void PacketHandlers()
    {
        PacketHandler(CommandKeys.LoginRequest, HandleLoginRequest);
        PacketHandler(CommandKeys.RegistrationRequest, HandleRegistrationRequest);
        PacketHandler(CommandKeys.GetPing, HandlePingRequest);
        Logger.Log("Packet handlers registered.", LogLevel.Debug);
    }

    private void PacketHandler(string keyType, Action<object> handler)
    {
        handlers[keyType] = handler;
        Logger.Log($"Packet handler registered for key: {keyType}", LogLevel.Debug);
    }

    private async void HandleLoginRequest(object dataObject)
    {
        if (!(dataObject is GlobalDataClasses.UserDataObject userData))
        {
            Logger.Log("Invalid login request data.", LogLevel.Warning);
            await answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.UnknownPackage);
            return;
        }

        string email = userData.Email;
        string hashedPassword = userData.HashedPassword;
        string hardwareID = userData.HardwareID;
        bool forceLoginRequested = userData.ForceLoginRequested;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(hardwareID))
        {
            Logger.Log("Login request with missing data.", LogLevel.Warning);
            await answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.IncorrectData);
            return;
        }

        bool loginResult = await dataHandler.HandleLoginDataAsync(email, hashedPassword, hardwareID, forceLoginRequested);

        if (!loginResult)
        {
            Logger.Log($"Failed login attempt for email: {email}", LogLevel.Warning);
            await answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.FailedLogin);
            return;
        }

        int userId = await dataHandler.GetUserIdByEmailAsync(email);
        if (!int.TryParse(await dataHandler.IsUserActiveAsync(email), out int status))
        {
            Logger.Log($"Failed to convert user status to integer for email: {email}", LogLevel.Error);
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
                        Logger.Log($"Forced login for email: {email}", LogLevel.Info);
                        await answerToClient.ServerResponseWrapper(CommandKeys.DisconnectKey, GlobalStrings.ForcedLogin);
                    }
                    else
                    {
                        Logger.Log($"Socket with number {currentActiveSocketNum} not found.", LogLevel.Error);
                    }
                }
            }
            else
            {
                Logger.Log($"User already online: {email}", LogLevel.Warning);
                await answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.UserIsAlreadyOnline);
                return;
            }
        }

        await dataHandler.SetClientStatusAsync(email, 1);
        await dataHandler.SetSocketClientAsync(email, clientSocketNum);
        await answerToClient.ServerResponseWrapper(CommandKeys.SuccessfulLogin, GlobalStrings.WelcomeMessage);
        Logger.Log($"User logged in successfully: {email}", LogLevel.Info);
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
                Logger.Log("Registration request with missing data.", LogLevel.Warning);
                await answerToClient.ServerResponseWrapper(CommandKeys.FailedRegistration, GlobalStrings.IncorrectData);
            }
            else
            {
                bool registerResult = await dataHandler.HandleRegistrationDataAsync(email, hashedPassword, hardwareID);

                if (registerResult)
                {
                    await answerToClient.ServerResponseWrapper(CommandKeys.SuccessfulRegistration, GlobalStrings.SuccessfulRegistration);
                    Logger.Log($"User registered successfully: {email}", LogLevel.Info);
                }
                else
                {
                    Logger.Log($"Registration error - email exists: {email}", LogLevel.Warning);
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
                Logger.Log($"Client status set to offline for email: {emailUserInLogged}", LogLevel.Info);
            }
        }
        catch (Exception ex)
        {
            Logger.Log($"Ошибка при отключении клиента: {ex.Message}", LogLevel.Error);
        }
    }

    public void GetClientSocket(string email)
    {
        currentActiveSocketNum = dataHandler.GetClientSocketNumAsync(email).Result;
        Logger.Log($"Retrieved client socket number for email: {email} - {currentActiveSocketNum}", LogLevel.Info);
    }

    private async void HandlePingRequest(object dataObject)
    {
        if (dataObject is GlobalDataClasses.RequestFromUser userRequest)
        {
            string pingValue = userRequest.GetPing;
            Logger.Log($"Received ping request with value: {pingValue}", LogLevel.Debug);

            await answerToClient.ServerResponseWrapper(CommandKeys.GetPing, GlobalStrings.GetPongMessage);
        }
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
                Logger.Log($"Processing packet with key: {keyType}", LogLevel.Debug);
                handler(dataObject);
            }
            else
            {
                Logger.Log($"No handler found for key: {keyType}", LogLevel.Warning);
            }
        });
    }
}