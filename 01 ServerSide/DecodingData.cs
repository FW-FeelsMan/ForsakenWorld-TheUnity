using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using ForsakenWorld;

public class DecodingData
{
    private readonly Dictionary<string, Action<object>> handlers = new();
    private readonly AnswerToClient answerToClient;
    private readonly DataHandler dataHandler = new();
    private readonly int clientSocketNum;

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

    private async void HandleLoginRequest(object dataObject)
    {
        if (!(dataObject is GlobalDataClasses.UserDataObject userData))
        {
            await answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.UnknownPackage);
            return;
        }

        string email = userData.Email;
        string hashedPassword = userData.HashedPassword;
        string hardwareID = userData.HardwareID;
        bool forceLoginRequested = userData.ForceLoginRequested;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(hardwareID))
        {
            await answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.IncorrectData);
            return;
        }

        bool loginResult = await dataHandler.HandleLoginDataAsync(email, hashedPassword, hardwareID, forceLoginRequested);

        if (!loginResult)
        {
            await answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.FailedLogin);
            return;
        }

        // Проверка статуса учетной записи пользователя
        string userStatus = await dataHandler.IsUserActiveAsync(email);
        if (userStatus == "1")
        {
            if (forceLoginRequested)
            {                
                await ForceDisconnectOtherClientsAsync(email);
            }
            else
            {
                await answerToClient.ServerResponseWrapper(CommandKeys.FailedLogin, GlobalStrings.UserIsAlreadyOnline);
                Logger.Log($"Login failed: user {email} is already online.", LogLevel.Warning);
                return;
            }
        }

        await dataHandler.SetClientStatusAsync(email, 1);
        await dataHandler.SetSocketClientAsync(email, clientSocketNum);
        await answerToClient.ServerResponseWrapper(CommandKeys.SuccessfulLogin, GlobalStrings.WelcomeMessage);
        Logger.Log($"Login successful for user {email}.", LogLevel.Info);
        
        var characters = await dataHandler.GetUserCharactersAsync(email);
        foreach (var character in characters)
        {
            string className = Character.GetClassName(character.ClassId);
            string genderName = Character.GetGenderName(character.GenderId);
            string raceName = Character.GetRaceName(character.RaceId);
            Logger.Log($"Character: {character.CharacterId}, Class: {className}, Gender: {genderName}, Race: {raceName}, Level: {character.Level}", LogLevel.Info);
        }
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
                await answerToClient.ServerResponseWrapper(CommandKeys.FailedRegistration, GlobalStrings.IncorrectData);
            }
            else
            {
                bool registerResult = await dataHandler.HandleRegistrationDataAsync(email, hashedPassword, hardwareID);

                if (registerResult)
                {
                    await answerToClient.ServerResponseWrapper(CommandKeys.SuccessfulRegistration, GlobalStrings.SuccessfulRegistration);
                }
                else
                {
                    await answerToClient.ServerResponseWrapper(CommandKeys.FailedRegistration, GlobalStrings.RegistrationErrorEmailExists);
                }
            }
        }
    }

    public async Task ClientisDisconnected()
    {
        try
        {
            var emailUserInLogged = dataHandler.GetLoggedInUserEmail();
            if (!string.IsNullOrEmpty(emailUserInLogged))
            {
                // Check if the disconnecting client is the current active session
                int currentSocketNum = await dataHandler.GetClientSocketNumAsync(emailUserInLogged);
                if (currentSocketNum == clientSocketNum)
                {
                    await dataHandler.SetClientStatusAsync(emailUserInLogged, 0);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Log($"Ошибка при отключении клиента: {ex.Message}", LogLevel.Error);
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
                handler(dataObject);
            }
        });
    }

    private async Task ForceDisconnectOtherClientsAsync(string email)
    {
        int currentSocketNum = await dataHandler.GetClientSocketNumAsync(email);
        var clientsToDisconnect = SocketServer.Instance.GetConnectedClients()
            .FindAll(client => ((int)client.Handle.ToInt64()) != currentSocketNum);

        foreach (var client in clientsToDisconnect)
        {
            SocketServer.Instance.RemoveClient(client);
            Logger.Log($"Client force removed: {client.RemoteEndPoint}", LogLevel.Info);
        }
    }
    
}
