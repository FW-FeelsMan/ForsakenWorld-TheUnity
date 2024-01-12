using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SocketClient : MonoBehaviour
{
    private static SocketClient instance;
    public static SocketClient Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SocketClient>();
            }
            return instance;
        }
    }

    private NetworkStream _networkStream;
    public event Action ServerDisconnected;
    public Socket Client { get; private set; }
    private bool isConnected = false;
    private UIManager uiManager;
    private PingManager pingManager;
    private ConnectionDataLoader connectionDataLoader;

    private void Start()
    {
        instance = this;
        uiManager = GetComponent<UIManager>();
        connectionDataLoader = gameObject.AddComponent<ConnectionDataLoader>();
        connectionDataLoader.LoadConnectionData();

        uiManager.CanvasLoadingScreen.enabled = false;
        uiManager.Menu_Environment.SetActive(false);
        pingManager = gameObject.AddComponent<PingManager>();
        pingManager.Initialize(connectionDataLoader.serverIpAddress);
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

    public async void SendData(byte[] data)
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

            _networkStream.Write(data, 0, data.Length);
        }
        catch (SocketException ex)
        {
            uiManager.DisplayError(GlobalStrings.ErrorConnectingToServer);
            Debug.Log(ex);
        }
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
                    string serverResponse = Encoding.UTF8.GetString(responseBuffer, 0, length);
                    Debug.Log($"Получен ответ от сервера: {serverResponse}");

                    //TODO
                   
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
