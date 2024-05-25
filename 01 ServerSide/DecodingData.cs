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
        Debug.Log($"Client disconnected event triggered for socket: {socketNum}");
    }

    public DecodingData(Socket clientSocket)
    {
        answerToClient = new AnswerToClient(clientSocket);
        IntPtr handle = clientSocket.Handle;
        clientSocketNum = handle.ToInt32();
        Debug.Log($"DecodingData initialized for client socket: {clientSocket.RemoteEndPoint}");
        PacketHandlers();
    }

    private void PacketHandlers()
    {
        PacketHandler(CommandKeys.LoginRequest, HandleLoginRequest);
        PacketHandler(CommandKeys.RegistrationRequest, HandleRegistrationRequest);
        PacketHandler(CommandKeys.GetPing, HandlePingRequest);
        Debug.Log("Packet handlers registered.");
    }

    private void PacketHandler(string keyType, Action<object> handler)
    {
        handlers[keyType] = handler;
        Debug.Log($"Packet handler registered for key: {keyType}");
    }

    private async void HandleLoginRequest(object dataObject)
    {
        if (!(dataObject is GlobalDataClasses.UserDataObject userData))
        {
            Debug.LogWarning("Invalid login request data.");
            await answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.UnknownPackage);
            return;
        }

        string email = userData.Email;
        string hashedPassword = userData.HashedPassword;
        string hardwareID = userData.HardwareID;
        bool forceLoginRequested = userData.ForceLoginRequested;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(hardwareID))
        {
            Debug.LogWarning("Login request with missing data.");
            await answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.IncorrectData);
            return;
        }

        bool loginResult = await dataHandler.HandleLoginDataAsync(email, hashedPassword, hardwareID, forceLoginRequested);

        if (!loginResult)
        {
            Debug.LogWarning($"Failed login attempt for email: {email}");
            await answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.FailedLogin);
            return;
        }

        int userId = await dataHandler.GetUserIdByEmailAsync(email);
        if (!int.TryParse(await dataHandler.IsUserActiveAsync(email), out int status))
        {
            Debug.LogError($"Failed to convert user status to integer for email: {email}");
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
                        Debug.Log($"Forced login for email: {email}");
                        await answerToClient.ServerResponseWrapper(CommandKeys.DisconnectKey, GlobalStrings.ForcedLogin);
                    }
                    else
                    {
                        Debug.LogError($"Socket with number {currentActiveSocketNum} not found.");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"User already online: {email}");
                await answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.UserIsAlreadyOnline);
                return;
            }
        }

        await dataHandler.SetClientStatusAsync(email, 1);
        await dataHandler.SetSocketClientAsync(email, clientSocketNum);
        await answerToClient.ServerResponseWrapper(CommandKeys.SuccessfulLogin, GlobalStrings.WelcomeMessage);
        Debug.Log($"User logged in successfully: {email}");
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
                Debug.LogWarning("Registration request with missing data.");
                await answerToClient.ServerResponseWrapper(CommandKeys.FailedRegistration, GlobalStrings.IncorrectData);
            }
            else
            {
                bool registerResult = await dataHandler.HandleRegistrationDataAsync(email, hashedPassword, hardwareID);

                if (registerResult)
                {
                    await answerToClient.ServerResponseWrapper(CommandKeys.SuccessfulRegistration, GlobalStrings.SuccessfulRegistration);
                    Debug.Log($"User registered successfully: {email}");
                }
                else
                {
                    Debug.LogWarning($"Registration error - email exists: {email}");
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
                Debug.Log($"Client status set to offline for email: {emailUserInLogged}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при отключении клиента: {ex.Message}");
        }
    }

    public void GetClientSocket(string email)
    {
        currentActiveSocketNum = dataHandler.GetClientSocketNumAsync(email).Result;
        Debug.Log($"Retrieved client socket number for email: {email} - {currentActiveSocketNum}");
    }

    private async void HandlePingRequest(object dataObject)
    {
        if (dataObject is GlobalDataClasses.RequestFromUser userRequest)
        {
            string pingValue = userRequest.GetPing;
            Debug.Log($"Received ping request with value: {pingValue}");

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
                Debug.Log($"Processing packet with key: {keyType}");
                handler(dataObject);
            }
            else
            {
                Debug.LogWarning($"No handler found for key: {keyType}");
            }
        });
    }
}