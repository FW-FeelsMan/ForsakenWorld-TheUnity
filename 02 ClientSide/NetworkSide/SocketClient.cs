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
    private ConnectionDataLoader connectionDataLoader;
    private DecodingDataFromServer decodingDataFromServer;

    private void Start()
    {
        uiManager = GetComponent<UIManager>();
        connectionDataLoader = gameObject.AddComponent<ConnectionDataLoader>();
        connectionDataLoader.LoadConnectionData();

        decodingDataFromServer = gameObject.AddComponent<DecodingDataFromServer>();

        uiManager.Initialization();
    }

    private void CloseConnection(Socket client)
    {
        if (client != null && client.Connected)
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
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
            uiManager.DisplayConnecting();

            if (!isConnected)
            {
                Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await ConnectToServerAsync();
                _networkStream = new NetworkStream(Client);
                StartCoroutine(CheckConnection(Client));
                StartListeningForResponses();
                isConnected = true;
            }

            await _networkStream.WriteAsync(data, 0, data.Length);

            if (!await IsPacketReceivedWithinTimeout())
            {
                uiManager.DisplayAnswer(0, GlobalStrings.ErrorWaitingForResponse);
            }
        }
        catch (SocketException ex)
        {
            uiManager.DisplayError(GlobalStrings.ErrorConnectingToServer);
            Debug.Log(ex);
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
                    Debug.Log($"Данные обнаружены: {bytesRead} байт");
                    return true;
                }
            }
            else
            {
                //Debug.Log($"Ожидание: {(DateTime.Now - startTime).TotalSeconds} секунд.");
                await Task.Delay(checkInterval);
            }
        }
        return false;
    }
    private async Task ConnectToServerAsync()
    {
        var connectTask = Task.Factory.FromAsync(Client.BeginConnect, Client.EndConnect, connectionDataLoader.serverIpAddress, connectionDataLoader.serverPort, null);
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
                    ServerDisconnected?.Invoke();
                    break;
                }
            }
            catch (Exception ex)
            {
                uiManager.DisplayError(GlobalStrings.ErrorMessageFailedReadData);
                Debug.Log($"{ex.Message}");
                ServerDisconnected?.Invoke();
                break;
            }
        }
    }

}
