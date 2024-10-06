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
    }

    private void CloseConnection(Socket client)
    {
        if (client != null && client.Connected)
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
            ThreadSafeLogger.Log("Connection closed.");
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
                
                await ConnectToServerAsync();
                _networkStream = new NetworkStream(Client);
                StartCoroutine(CheckConnection(Client));
                StartListeningForResponses();
                isConnected = true;
                uiManager.HideConnecting();
                
            }

            await _networkStream.WriteAsync(data, 0, data.Length);
        
        }
        catch (SocketException ex)
        {
            uiManager.DisplayError(GlobalStrings.ErrorConnectingToServer);
            ThreadSafeLogger.Log($"SocketException: {ex.Message}");
        }
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
        
    }

    private IEnumerator CheckConnection(Socket client)
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (_networkStream == null || !_networkStream.CanRead || !_networkStream.CanWrite)
            {
                uiManager.DisplayError(GlobalStrings.ErrorMessageConnectionLost);
                ThreadSafeLogger.Log("Connection lost.");
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
                    
                    await decodingDataFromServer.ProcessPacketAsync(responseBuffer);
                }
                else
                {
                    uiManager.DisplayError(GlobalStrings.ErrorMessageConnectionLost);
                    ThreadSafeLogger.Log("Connection lost: no data received.");
                    ServerDisconnected?.Invoke();
                    break;
                }
            }
            catch (Exception ex)
            {
                uiManager.DisplayError(GlobalStrings.ErrorMessageConnectionLost);
                ThreadSafeLogger.Log($"Exception while listening for responses: {ex.Message}");
                ServerDisconnected?.Invoke();
                break;
            }
        }
    }
}