using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SocketServer : Singleton<SocketServer>
{
    public bool isOn;
    private bool _isListening;
    private Socket _listener;
    private readonly int _port = 26950;
    public static List<Socket> connectedClients = new();
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
        _listener.Bind(new IPEndPoint(IPAddress.Any, _port));
        _listener.Listen(100);
        _isListening = true;

        ListenForClients();
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

                DecodingData decodingData = new(client);

                HandleClient(client, decodingData);
            }
            catch (ObjectDisposedException ex)
            {
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
                        _ = Task.Run(() => decodingData.ProcessPacketAsync(buffer));
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                    break;
                }

                await Task.Delay(1000);
            }
        }
        else
        {
           Debug.Log("Не удалось подключиться к клиенту");
        }
    }

    private void AddClient(Socket client)
    {
        connectedClients.Add(client);
    }

    public void RemoveClient(Socket client)
    {
        connectedClients.Remove(client);
        client.Close();
    }
    public void ForceRemoveClient(int socketNum)
    {
        Socket clientToRemove = connectedClients.Find(client => ((int)client.Handle.ToInt64()) == socketNum);
        if (clientToRemove != null)
        {
            connectedClients.Remove(clientToRemove);
            clientToRemove.Close();
        }
    }


    private void OnDestroy()
    {
        StopServer();
    }
}
