using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

public class SocketServer : Singleton<SocketServer>
{
    public bool isOn;
    private bool _isListening;
    private Socket _listener;
    private readonly int _port = 26950;
    private List<Socket> connectedClients = new();

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
        try
        {
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(new IPEndPoint(IPAddress.Any, _port));
            _listener.Listen(100);
            _isListening = true;
            Logger.Log("Server started and listening on port " + _port, LogLevel.Info);
            ListenForClients();
        }
        catch (Exception ex)
        {
            Logger.Log($"Error starting server: {ex.Message}", LogLevel.Error);
        }
    }

    private void StopServer()
    {
        if (_listener != null)
        {
            _isListening = false;
            _listener.Close();
            _listener = null;
            Logger.Log("Server stopped.", LogLevel.Info);
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

            Logger.Log("Client connected: " + client.RemoteEndPoint, LogLevel.Info);
            DecodingData decodingData = new(client);

                // Обработка каждого клиента в отдельной задаче
                _ = Task.Run(() => HandleClientAsync(client, decodingData));
        }
        catch (ObjectDisposedException ex)
        {
            Logger.Log($"Server stopped listening for clients: {ex.Message}", LogLevel.Warning);
        }
        catch (Exception ex)
        {
            Logger.Log($"Error accepting client: {ex.Message}", LogLevel.Error);
        }
    }
}

private async Task HandleClientAsync(Socket client, DecodingData decodingData)
{
    if (client != null && client.Connected)
    {
        using NetworkStream networkStream = new(client);
        using BinaryReader reader = new(networkStream);

        AddClient(client);

        while (client != null && client.Connected)
        {
            try
            {
                bool isDisconnected = client.Poll(1000, SelectMode.SelectRead) && client.Available == 0;
                if (isDisconnected)
                {
                    Logger.Log($"Client disconnected: {client.RemoteEndPoint}", LogLevel.Warning);
                    await RemoveClientAsync(client, decodingData);
                    break;
                }

                byte[] buffer = new byte[1024];
                int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    Logger.Log($"Received {bytesRead} bytes from {client.RemoteEndPoint}", LogLevel.Debug);
                    _ = Task.Run(() => decodingData.ProcessPacketAsync(buffer));
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error handling client {client.RemoteEndPoint}: {ex.Message}", LogLevel.Error);
                await RemoveClientAsync(client, decodingData);
                break;
            }

            await Task.Delay(1000);
        }
    }
    else
    {
        Logger.Log("Failed to connect to client.", LogLevel.Warning);
    }
}


    private void AddClient(Socket client)
    {
        if (!connectedClients.Contains(client))
        {
            connectedClients.Add(client);
            Logger.Log("Client added: " + client.RemoteEndPoint, LogLevel.Info);
        }
    }

    public async Task RemoveClientAsync(Socket client, DecodingData decodingData)
    {
        await decodingData.ClientisDisconnected();
        connectedClients.Remove(client);
        client.Close();
        Logger.Log("Client removed: " + client.RemoteEndPoint, LogLevel.Info);
    }

    public void RemoveClient(Socket client)
    {
        connectedClients.Remove(client);
        client.Close();
        Logger.Log("Client removed: " + client.RemoteEndPoint, LogLevel.Info);
    }

    public List<Socket> GetConnectedClients()
    {
        return new List<Socket>(connectedClients);
    }

    private void OnDestroy()
    {
        StopServer();
    }
}
