using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using ForsakenWorldLIB.Unity.GetCallingClassName;

public class SocketServer : MonoBehaviour
{
    public bool isOn;
    private Socket _listener;
    private bool _isListening;
    private static SocketServer instance;
    string thisClassName;
    private List<Socket> connectedClients = new();
    public static SocketServer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SocketServer>();
            }
            return instance;
        }
    }

    private void Start()
    {
        if (isOn)
        {
            StartServer();
        }
        thisClassName = FWLibrary.GetCallingClassName();
    }

    private void Update()
    {
        if (isOn && !_isListening)
        {
            StartServer();
        }
        else if (!isOn && _isListening)
        {
            StopServer();
        }
    }

    private void StartServer()
    {
        _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _listener.Bind(new IPEndPoint(IPAddress.Any, 26950));
        _listener.Listen(100);
        _isListening = true;

        ListenForClients();

        LogProcessor.ProcessLog(thisClassName, GlobalStrings.ServerStarted);
    }

    private void StopServer()
    {
        if (_listener != null)
        {
            _isListening = false;
            _listener.Close();
            _listener = null;
        }
    }

    private async void ListenForClients()
    {
        while (_isListening && _listener != null)
        {
            try
            {
                Socket client = await Task.Factory.FromAsync(
                    _listener.BeginAccept,
                    _listener.EndAccept,
                    null
                );

                DecodingData decodingData = new();

                HandleClient(client, decodingData);
            }
            catch (ObjectDisposedException ex)
            {
                LogProcessor.ProcessLog(thisClassName, ex);
                Debug.Log(ex);
            }
        }
    }

    private async void HandleClient(Socket client, DecodingData decodingData)
    {
        if (client != null && client.Connected)
        {
            using NetworkStream networkStream = new(client);
            using BinaryReader reader = new(networkStream);
            while (client != null && client.Connected)
            {
                try
                {
                    bool isDisconnected = client.Poll(1000, SelectMode.SelectRead) && client.Available == 0;
                    if (isDisconnected)
                    {
                        decodingData.ClientisDisconnected();
                        RemoveClient(client);                        
                        break;
                    }

                    byte[] buffer = new byte[1024];
                    int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                    AddClient(client);
                    if (bytesRead > 0)
                    {
                        decodingData.ProcessPacketAsync(buffer);
                    }
                }
                catch (Exception ex)
                {
                    LogProcessor.ProcessLog(thisClassName, "Ошибка при обработке клиента: " + ex.Message);
                    Debug.LogError(ex);
                    break;
                }
                await Task.Delay(1000);
            }
        }
        else
        {
            LogProcessor.ProcessLog(thisClassName, "Не удалось подключиться к клиенту");
        }
    }
    private void AddClient(Socket client)
    {
        connectedClients.Add(client);
    }

    private void RemoveClient(Socket client)
    {
        connectedClients.Remove(client);
        client.Close();
    }

    public async Task SendDataAsync(byte[] data)
    {
        foreach (var client in connectedClients)
        {
            try
            {
                using NetworkStream networkStream = new(client);
                await networkStream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                LogProcessor.ProcessLog(thisClassName, $"Ошибка при отправке данных клиенту: {ex.Message}");
                Debug.LogError(ex);
            }
        }
    }

    private void OnDestroy()
    {
        StopServer();
    }
}
