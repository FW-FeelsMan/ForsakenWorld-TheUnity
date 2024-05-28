// File: SocketClient.cs
using System;
using System.Collections;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class SocketClient : Singleton<SocketClient>
{
    private NetworkStream _networkStream;
    public event Action ServerDisconnected;
    public Socket Client { get; private set; }
    private bool isConnected = false;
    private UIManager uiManager;
    private DecodingDataFromServer decodingDataFromServer;

    private void Start()
    {
        uiManager = GetComponent<UIManager>();
        ConnectionDataLoader.LoadConnectionData();

        decodingDataFromServer = gameObject.AddComponent<DecodingDataFromServer>();

        uiManager.Initialization();
        Logger.CurrentLogLevel = LogLevel.Debug;  // Set the desired log level here
    }

    private void CloseConnection(Socket client)
    {
        if (client != null && client.Connected)
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
            Logger.Log("Connection closed.", LogLevel.Info);
        }
    }

    private void OnDestroy()
    {
        if (_networkStream != null)
        {
            CloseConnection(Client);
        }
    }

    private void OnApplicationQuit()
    {
        if (_networkStream != null)
        {
            CloseConnection(Client);
        }
    }

    public async Task SendData(byte[] data)
    {
        try
        {
            if (!isConnected)
            {
                Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                uiManager.DisplayConnecting();
                Logger.Log("Connecting to server...", LogLevel.Info);
                await ConnectToServerAsync();
                _networkStream = new NetworkStream(Client);
                StartCoroutine(CheckConnection(Client));
                StartListeningForResponses();
                isConnected = true;
                uiManager.HideConnecting();
                Logger.Log("Connected to server.", LogLevel.Info);
            }

            await _networkStream.WriteAsync(data, 0, data.Length);
            Logger.Log($"Sent {data.Length} bytes to server.", LogLevel.Debug);

            /*if (!await IsPacketReceivedWithinTimeout())
            {
                uiManager.DisplayAnswer(0, GlobalStrings.ErrorWaitingForResponse);
            }*/
        }
        catch (SocketException ex)
        {
            uiManager.DisplayError(GlobalStrings.ErrorConnectingToServer);
            Logger.Log($"SocketException: {ex.Message}", LogLevel.Error);
        }
    }

    private async Task<bool> IsPacketReceivedWithinTimeout()
    {
        int timeoutMilliseconds = 5000;
        int checkInterval = 100; 
        DateTime startTime = DateTime.Now;

        while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMilliseconds)
        {
            if (_networkStream.DataAvailable)
            {
                byte[] buffer = new byte[1024]; 
                int bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    Logger.Log($"Received {bytesRead} bytes from server.", LogLevel.Debug);
                    return true;
                }
            }
            else
            {
                await Task.Delay(checkInterval);
            }
        }
        Logger.Log("Timeout waiting for packet from server.", LogLevel.Warning);
        return false;
    }

    private async Task ConnectToServerAsync()
    {
        var connectTask = Task.Factory.FromAsync(Client.BeginConnect, Client.EndConnect, ConnectionDataLoader.serverIpAddress, ConnectionDataLoader.serverPort, null);
        await connectTask.ConfigureAwait(false);

        if (!Client.Connected)
        {
            isConnected = false;
            throw new SocketException((int)SocketError.TimedOut);
        }
        Logger.Log("Successfully connected to server.", LogLevel.Info);
    }

    private IEnumerator CheckConnection(Socket client)
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (_networkStream == null || !_networkStream.CanRead || !_networkStream.CanWrite)
            {
                uiManager.DisplayError(GlobalStrings.ErrorMessageConnectionLost);
                Logger.Log("Connection lost.", LogLevel.Warning);
                ServerDisconnected?.Invoke();
                break;
            }
        }
    }

    public async void StartListeningForResponses()
    {
        byte[] responseBuffer = new byte[1024];

        while (Client != null && Client.Connected)
        {
            try
            {
                var length = await _networkStream.ReadAsync(responseBuffer, 0, responseBuffer.Length);

                if (length > 0)
                {
                    Logger.Log($"Received {length} bytes from server.", LogLevel.Debug);
                    await decodingDataFromServer.ProcessPacketAsync(responseBuffer);
                }
                else
                {
                    uiManager.DisplayError(GlobalStrings.ErrorMessageConnectionLost);
                    Logger.Log("Connection lost: no data received.", LogLevel.Warning);
                    ServerDisconnected?.Invoke();
                    break;
                }
            }
            catch (Exception ex)
            {
                uiManager.DisplayError(GlobalStrings.ErrorMessageConnectionLost);
                Logger.Log($"Exception while listening for responses: {ex.Message}", LogLevel.Error);
                ServerDisconnected?.Invoke();
                break;
            }
        }
    }
}
