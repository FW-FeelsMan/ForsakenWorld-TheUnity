using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class AnswerToClient : MonoBehaviour
{
    private readonly Socket clientSocket;

    public AnswerToClient(Socket clientSocket)
    {
        this.clientSocket = clientSocket;
    }

    public async Task ServerResponseWrapper(string keyType, string message)
    {
        await ServerResponseMessageObject(keyType, message);
    }

    public async Task ServerResponseMessageObject(string keyType, string message)
    {
        var dataObject = new GlobalDataClasses.ServerResponseMessage
        {
            KeyType = keyType,
            Message = message
        };  

        await RequestTypeAsync(keyType, dataObject);
    }

    private async Task RequestTypeAsync(string keyType, object dataObject)
    {
        try
        {
            byte[] requestData;
            var formatter = new BinaryFormatter();

            using (MemoryStream memoryStream = new())
            {
                formatter.Serialize(memoryStream, keyType);
                formatter.Serialize(memoryStream, dataObject);

                memoryStream.Position = 0;
                string debugKeyType = (string)formatter.Deserialize(memoryStream);
                object debugDataObject = formatter.Deserialize(memoryStream);

                await memoryStream.FlushAsync();
                requestData = memoryStream.ToArray();
            }

            await SendDataAsync(requestData);
        }
        catch (Exception ex)
        {
            ThreadSafeLogger.Log($"Error in RequestTypeAsync: {ex.Message}");
            CloseClientSocket();
        }
    }

    private async Task SendDataAsync(byte[] data)
    {
        try
        {
            using NetworkStream networkStream = new(clientSocket);
            await networkStream.WriteAsync(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            ThreadSafeLogger.Log($"Error sending data: {ex.Message}");
            CloseClientSocket();
        }
    }

    private void CloseClientSocket()
    {
        try
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            ThreadSafeLogger.Log("Client socket closed.");
        }
        catch (Exception ex)
        {
            ThreadSafeLogger.Log($"Error closing socket: {ex.Message}");
        }
    }
}